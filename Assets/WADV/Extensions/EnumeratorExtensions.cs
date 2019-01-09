using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using WADV.Thread;
using UnityEngine;

namespace WADV.Extensions {
    public static class EnumeratorExtensions {
        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest instruction) {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => CoroutineRunner.Instance.StartCoroutine(ResourceRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction) {
            var awaiter = new SimpleCoroutineAwaiter<AssetBundle>();
            RunOnUnityScheduler(() => CoroutineRunner.Instance.StartCoroutine(AssetBundleCreateRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest instruction) {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => CoroutineRunner.Instance.StartCoroutine(AssetBundleRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<T> GetAwaiter<T>(this IEnumerator<T> coroutine) {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => CoroutineRunner.Instance.StartCoroutine(new CoroutineWrapper<T>(coroutine, awaiter).Run()));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<object> GetAwaiter(this IEnumerator coroutine) {
            var awaiter = new SimpleCoroutineAwaiter<object>();
            RunOnUnityScheduler(() => CoroutineRunner.Instance.StartCoroutine(new CoroutineWrapper<object>(coroutine, awaiter).Run()));
            return awaiter;
        }
        
        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForSeconds instruction) {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForUpdate instruction) {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForEndOfFrame instruction) {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForFixedUpdate instruction) {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitForSecondsRealtime instruction) {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitUntil instruction) {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter GetAwaiter(this WaitWhile instruction) {
            return GetAwaiterReturnVoid(instruction);
        }

        public static SimpleCoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation instruction) {
            return GetAwaiterReturnSelf(instruction);
        }
        
#pragma warning disable 618
        public static SimpleCoroutineAwaiter<WWW> GetAwaiter(this WWW instruction) {
            return GetAwaiterReturnSelf(instruction);
        }
#pragma warning restore 618

        private static SimpleCoroutineAwaiter GetAwaiterReturnVoid(object instruction) {
            var awaiter = new SimpleCoroutineAwaiter();
            RunOnUnityScheduler(() => CoroutineRunner.Instance.StartCoroutine(ReturnVoid(awaiter, instruction)));
            return awaiter;
        }

        private static SimpleCoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T instruction) {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => CoroutineRunner.Instance.StartCoroutine(ReturnSelf(awaiter, instruction)));
            return awaiter;
        }

        private static void RunOnUnityScheduler(Action action) {
            if (SynchronizationContext.Current == CoroutineRunner.MainThreadContext) {
                action();
            } else {
                CoroutineRunner.MainThreadContext.Post(_ => action(), null);
            }
        }

        public class SimpleCoroutineAwaiter<T> : INotifyCompletion {
            private Exception _exception;
            private Action _continuation;
            private T _result;

            public bool IsCompleted { get; private set; }

            public T GetResult() {
                if (!IsCompleted) {
                    throw new NotSupportedException("Task is not complete");
                }
                if (_exception != null) {
                    ExceptionDispatchInfo.Capture(_exception).Throw();
                }
                return _result;
            }

            public void Complete(T result, Exception e) {
                if (IsCompleted) {
                    throw new NotSupportedException("Task is already completed");
                }
                IsCompleted = true;
                _exception = e;
                _result = result;
                if (_continuation != null) {
                    RunOnUnityScheduler(_continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation) {
                if (_continuation != null || IsCompleted) {
                    throw new NotSupportedException("Task is already completed");
                }
                _continuation = continuation;
            }
        }

        public class SimpleCoroutineAwaiter : INotifyCompletion {
            private Exception _exception;
            private Action _continuation;

            public bool IsCompleted { get; private set; }

            public void GetResult() {
                if (!IsCompleted) {
                    throw new NotSupportedException("Task is not complete");
                }
                if (_exception != null) {
                    ExceptionDispatchInfo.Capture(_exception).Throw();
                }
            }

            public void Complete(Exception e) {
                if (IsCompleted) {
                    throw new NotSupportedException("Task is already completed");
                }
                IsCompleted = true;
                _exception = e;
                if (_continuation != null) {
                    RunOnUnityScheduler(_continuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action continuation) {
                if (_continuation != null || IsCompleted) {
                    throw new NotSupportedException("Task is already completed");
                }
                _continuation = continuation;
            }
        }

        private class CoroutineWrapper<T> {
            private readonly SimpleCoroutineAwaiter<T> _awaiter;
            private readonly Stack<IEnumerator> _processStack;

            public CoroutineWrapper(IEnumerator coroutine, SimpleCoroutineAwaiter<T> awaiter) {
                _processStack = new Stack<IEnumerator>();
                _processStack.Push(coroutine);
                _awaiter = awaiter;
            }

            public IEnumerator Run() {
                while (true) {
                    var worker = _processStack.Peek();
                    bool isDone;
                    try {
                        isDone = !worker.MoveNext();
                    } catch (Exception e) {
                        _awaiter.Complete(default(T), e);
                        yield break;
                    }
                    if (isDone) {
                        _processStack.Pop();
                        if (_processStack.Count == 0) {
                            _awaiter.Complete((T) worker.Current, null);
                            yield break;
                        }
                    }
                    if (worker.Current is IEnumerator enumerator) {
                        _processStack.Push(enumerator);
                    } else {
                        yield return worker.Current;
                    }
                }
            }
        }
        
        private static IEnumerator ReturnVoid(SimpleCoroutineAwaiter awaiter, object instruction) {
            yield return instruction;
            awaiter.Complete(null);
        }

        private static IEnumerator AssetBundleCreateRequest(SimpleCoroutineAwaiter<AssetBundle> awaiter, AssetBundleCreateRequest instruction) {
            yield return instruction;
            awaiter.Complete(instruction.assetBundle, null);
        }

        private static IEnumerator ReturnSelf<T>(SimpleCoroutineAwaiter<T> awaiter, T instruction) {
            yield return instruction;
            awaiter.Complete(instruction, null);
        }

        private static IEnumerator AssetBundleRequest(SimpleCoroutineAwaiter<UnityEngine.Object> awaiter, AssetBundleRequest instruction) {
            yield return instruction;
            awaiter.Complete(instruction.asset, null);
        }

        private static IEnumerator ResourceRequest(SimpleCoroutineAwaiter<UnityEngine.Object> awaiter, ResourceRequest instruction) {
            yield return instruction;
            awaiter.Complete(instruction.asset, null);
        }
    }
}