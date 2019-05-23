using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using WADV.Thread;
using UnityEngine;

namespace WADV.Extensions {
    public static class AsyncExtensions {
        public static TaskAwaiter<int> GetAwaiter(this Process process) {
            var source = new TaskCompletionSource<int>();
            if (process.HasExited) {
                source.TrySetResult(process.ExitCode);
            } else {
                process.EnableRaisingEvents = true;
                process.Exited += (s, e) => source.TrySetResult(process.ExitCode);
            }
            return source.Task.GetAwaiter();
        }

        public static async void WrapErrors(this Task task) {
            await task;
        }
        
        public static async Task<T> WrapErrors<T>(this Task<T> task) {
            return await task;
        }
        
        public static IEnumerator AsIEnumerator(this Task task) {
            while (!task.IsCompleted) {
                yield return null;
            }
            if (task.IsFaulted) {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
        }

        public static IEnumerator<T> AsIEnumerator<T>(this Task<T> task) {
            while (!task.IsCompleted) {
                yield return default;
            }
            if (task.IsFaulted) {
                ExceptionDispatchInfo.Capture(task.Exception).Throw();
            }
            yield return task.Result;
        }

        /// <summary>
        /// 等待任务执行结束并返回执行结果
        /// </summary>
        /// <param name="task">目标任务</param>
        /// <returns></returns>
        public static T GetResultAfterFinished<T>(this Task<T> task) {
            task.Wait();
            return task.Result;
        }
        
        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest instruction) {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => TaskDelegator.Instance.StartCoroutine(ResourceRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest instruction) {
            var awaiter = new SimpleCoroutineAwaiter<AssetBundle>();
            RunOnUnityScheduler(() => TaskDelegator.Instance.StartCoroutine(AssetBundleCreateRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest instruction) {
            var awaiter = new SimpleCoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => TaskDelegator.Instance.StartCoroutine(AssetBundleRequest(awaiter, instruction)));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<T> GetAwaiter<T>(this IEnumerator<T> coroutine) {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => TaskDelegator.Instance.StartCoroutine(new CoroutineWrapper<T>(coroutine, awaiter).Run()));
            return awaiter;
        }

        public static SimpleCoroutineAwaiter<object> GetAwaiter(this IEnumerator coroutine) {
            var awaiter = new SimpleCoroutineAwaiter<object>();
            RunOnUnityScheduler(() => TaskDelegator.Instance.StartCoroutine(new CoroutineWrapper<object>(coroutine, awaiter).Run()));
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
            RunOnUnityScheduler(() => TaskDelegator.Instance.StartCoroutine(ReturnVoid(awaiter, instruction)));
            return awaiter;
        }

        private static SimpleCoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T instruction) {
            var awaiter = new SimpleCoroutineAwaiter<T>();
            RunOnUnityScheduler(() => TaskDelegator.Instance.StartCoroutine(ReturnSelf(awaiter, instruction)));
            return awaiter;
        }

        private static void RunOnUnityScheduler(Action action) {
            if (SynchronizationContext.Current == TaskDelegator.MainThreadContext) {
                action();
            } else {
                TaskDelegator.MainThreadContext.Post(_ => action(), null);
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
                        _awaiter.Complete(default, e);
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