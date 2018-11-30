;参数笔记
;这些都不是真实数据，仅用于阅读此文件
;角色[0,0,0,63]
;0 编号

;对话[0,0,0,80]
;0 角色 1 内容 2 来源 3 广播
;0 广播

;样式[0,0,0,169]
;0 编号 1 颜色 2 基础样式 3 无变量

;切换语言[0,0,0,151]
;0 目标

;提示[0,0,0,147]
;0 内容

;声音[0,0,0,40]
;0 声道 1 资源 2 淡入 3 淡出 4 时长 5 循环

;对象[0,0,0,221]
;0......

;指令表
;任意指令如果对栈顶操作，那么一定会出栈栈顶元素且不做特殊说明不再放入
;无宏模式变量的地方尽可能把指令识别符、参数编号、子元素编号等等直接放入以免去运行时根据字符串查找指令的过程
;一级指令（8bit XXXXXXXX）
;ldc.i4.0   入栈32位有符号数字0
;ldc.i4.1   入栈32位有符号数字1
;ldc.i4.2   入栈32位有符号数字2
;ldc.i4.3   入栈32位有符号数字3
;ldc.i4.4   入栈32位有符号数字4
;ldc.i4.5   入栈32位有符号数字5
;ldc.i4.6   入栈32位有符号数字6
;ldc.i4.7   入栈32位有符号数字7
;ldc.i4.8   入栈32位有符号数字8
;ldc.i4     入栈32位有符号数字
;ldc.r4.0   入栈32位浮点数0.0
;ldc.r4.025 入栈32位浮点数0.25
;ldc.r4.05  入栈32位浮点数0.5
;ldc.r4.075 入栈32位浮点数0.75
;ldc.r4.1   入栈32位浮点数1.0
;ldc.r4.125 入栈32位浮点数1.25
;ldc.r4.15  入栈32位浮点数1.5
;ldc.r4.175 入栈32位浮点数1.75
;ldc.r4.2   入栈32位浮点数2.0
;ldc.r4.225 入栈32位浮点数2.25
;ldc.r4.25  入栈32位浮点数2.5
;ldc.r4.275 入栈32位浮点数2.75
;ldc.r4.3   入栈32位浮点数3.0
;ldc.r4.325 入栈32位浮点数3.25
;ldc.r4.35  入栈32位浮点数3.5
;ldc.r4.375 入栈32位浮点数3.75
;ldc.r4.4   入栈32位浮点数4.0
;ldc.r4.425 入栈32位浮点数4.25
;ldc.r4.45  入栈32位浮点数4.5
;ldc.r4.475 入栈32位浮点数4.75
;ldc.r4.5   入栈32位浮点数5.0
;ldc.r4     入栈32位浮点数
;lduid      入栈128位System.Guid数字
;ldstr      入栈字符串
;ldstt      入栈可翻译字符串
;ldnul      入栈null
;ldloc      读取栈顶字符串对应的变量并将其值入栈
;ldt        入栈true
;ldf        入栈false
;call       调用指令，此时栈顶必须是指令名
;pop        出栈
;dialogue   使用栈顶的两个元素生成对话
;del        删除变量，实际等价于ldnul + ldstr var_name + stloc + pop
;add        相加栈顶两个元素后入栈
;sub        相减栈顶两个元素后入栈
;mul        相乘栈顶两个元素后入栈
;div        相除栈顶两个元素后入栈
;not        对栈顶元素真值取反
;eql        比较栈顶两个元素是否真值相等
;cge        比较栈顶两个元素，如果栈顶元素大于等于下一个栈元素则入栈true，否则入栈false
;cgt        比较栈顶两个元素，如果栈顶元素大于下一个栈元素则入栈true，否则入栈false
;cle        比较栈顶两个元素，如果栈顶元素小于等于下一个栈元素则入栈true，否则入栈false
;clt        比较栈顶两个元素，如果栈顶元素小于等于下一个栈元素则入栈true，否则入栈false
;stloc      将栈顶值放入栈顶字符串对应的变量，并将该值重新入栈
;pick       取栈顶元素中下一个栈元素对于的子元素
;scope      创建作用域
;leave      销毁顶层作用域
;empty      指令占位符
;lang       根据栈顶字符串切换指令语言
;bf.s       如果栈顶元素真值为false则跳转
;bt.s       如果栈顶元素真值为true则跳转
;br.s       无条件跳转
;bge.s      如果栈顶元素大于等于下一个栈元素则跳转
;bgt.s      如果栈顶元素大于下一个栈元素则跳转
;ble.s      如果栈顶元素小于等于下一个栈元素则跳转
;blt.s      如果栈顶元素小于下一个栈元素则跳转
;be.s       如果栈顶元素等于下一个栈元素则跳转
;bne.s      如果栈顶元素不等于下一个栈元素则跳转

;所有""内的字符串以及对话内容（非角色）都应该进入翻译文件中

.label __func_引入场景__ empty
.label __func_引入场景_子场景1__ ret
;[声音->播放 声道=背景音1 资源="Assets/Resources/Intro" 淡入=2.0 循环]
ldstr 背景音1
ldc.i4.0
ldstt 6 ;Assets/Resources/Intro
ldc.i4.1
ldc.r4.2
ldc.i4.2
ldnul
ldc.i4.5
ldc.i4.4
lduid 0,0,0,40
call
pop
;[声音->暂停 声道=背景音1 时长=0.25]
ldc.r4.025
ldc.i4.4
ldstr 背景音1
ldc.i4.0
ldc.i4.2
ldstr 暂停
lduid 187,209,68,40,152,67,188,70,155,128,208,185,139,204,73,61
pick
call
pop
;#小红 测试对话2。
ldstt 8 ;测试对话2。
ldstt 7 ;小红
dialogue
;[@"场景"->执行 目标=子场景1]
ldstr 子场景1
ldstr 目标
ldc.i4.1
ldstr 执行
ldstt 9 ;场景
ldloc
pick
call
pop
;["场景"->执行 目标=测试1 @变量1=1 @变量2=2]
ldc.i4.2
ldstr 变量2
ldloc
ldc.i4.1
ldstr 变量1
ldloc
ldstr 测试1
ldstr 目标
ldc.i4.3
ldstr 执行
ldstt 10 ;场景
pick
call

ret

.label __func_测试1__ ldstr 变量2
ldnul
ldstr 变量2
ldloc
bne.s __func_测试1_content__
ldc.i4.1
ldstr 变量2
stloc
pop
.label __func_测试1_content__ ldloc
ldstr 变量1
ldloc
add
ldstr 变量1
stloc
pop
ldstr 变量2
ldloc
ldstr 变量1
ldloc
add
;作用域清理
ldstr 变量1
del
ldstr 变量2
del
ret

.label __func_test1__ ldstr var2
ldloc
ldstr var1
ldloc
add
ldstr var1
stloc
pop
ldstr var2
ldloc
ldstr var1
ldloc
add
;作用域清理
ldstr var1
del
ldstr var2
del
ret

;;[角色 编号=旁白]
.entrypoint ldstr 旁白 ;第一个参数的内容
ldc.i4.0 ;第一个参数的索引
ldc.i4.1 ;共一个参数
lduid 239,250,109,63,53,13,222,72,164,103,144,156,132,215,139,36 ;指令唯一识别码
call ;调用栈顶uuid所指的指令
pop ;出栈指令结果

;;[角色 编号="小红"]
ldstt 0 ;小红
ldc.i4.0
ldc.i4.1
lduid 239,250,109,63,53,13,222,72,164,103,144,156,132,215,139,36
call
pop

;;#旁白 这是隐藏对话。\n因为[粗体]对话框[取消粗体]没有显示。[不暂停]
ldstt 2 ;这是隐藏对话。\n因为[粗体]对话框[取消粗体]没有显示。[不暂停]
ldstt 1 ;旁白
dialogue

;;\+ 这是普通的对话。
ldstt 3 ;\+ 这是普通的对话。
ldnul ;null
dialogue

;;[对话 角色=旁白 内容=("这里可以做各种运算了" + ((@系统语言 + 1) * 2)) 广播]
ldstt 旁白
ldc.i4.0
ldc.i4.2
ldc.i4.1
ldstr 系统语言
ldloc
add
mul
ldstt 4 ;这里可以做各种运算了
add
ldc.i4.1
ldnul
ldc.i4.3
ldc.i4.3
lduid 172,161,172,80,69,70,23,74,189,108,80,216,33,199,202,240
call
pop

;;@测试对话 = [对话 内容=也可以这么用]
ldstr 也可以这么用
ldc.i4.1
ldc.i4.1
lduid 172,161,172,80,69,70,23,74,189,108,80,216,33,199,202,240
call
ldstr 测试对话
stloc
pop

;;[对话->@测试对话 来源=@测试对话]
ldstr 测试对话
ldloc
ldc.i4.2
idc.i4.1
ldstr 测试对话
ldloc
lduid 172,161,172,80,69,70,23,74,189,108,80,216,33,199,202,240
pick
call
pop

;;@@测试对话 = @null
ldnul
ldstr 测试对话
ldloc
stloc
pop

;;[对话->广播 来源=[对话 角色=旁白 内容=更高级的用法]]
ldstr 更高级的用法
ldc.i4.1
ldstr 旁白
ldc.i4.0
ldc.i4.2
lduid 172,161,172,80,69,70,23,74,189,108,80,216,33,199,202,240
call
ldc.i4.2
ldc.i4.1
ldc.i4.0
lduid 172,161,172,80,69,70,23,74,189,108,80,216,33,199,202,240
pick
call
pop

;;@基础样式 = @null
ldnul
ldstr 基础样式
stloc
pop

scope

;;@基础样式 = (;如果 (@系统语言 == zh_CN) + !@系统语言
;;    [样式 编号=基础样式 颜色=#808080 无变量l]
;;;否则
;;    [样式 编号=基础样式 颜色=#222222 无变量]
;;) + @基础样式
ldstr 基础样式
ldloc
ldstr 系统语言
ldloc
not
ldstr zh-CN
ldstr 系统语言
ldloc
eql
add
bf.s __cond_0__
ldstr 基础样式
ldc.i4.0
ldstr #808080
ldc.i4.1
ldnul
ldc.i4.3
ldc.i4.3
lduid 15,249,2,169,230,153,88,75,146,78,70,173,53,24,242,136
call
br.s __cond_1__
.label __cond_0__ ldstr 基础样式
ldc.i4.0
ldstr #222222
ldc.i4.1
ldnul
ldc.i4.3
ldc.i4.3
lduid 15,249,2,169,230,153,88,75,146,78,70,173,53,24,242,136
call
.label __cond_1__ add
ldstr 基础样式
stloc
pop

leave

;;;如果 @@系统语言==zh_CN
;;    [样式 基础样式=@样式#对话框主体_简中]
;;    ;如果 @初次运行==@true
;;        @事件列表 = [对象 0=@false 1=@false]
;;        @事件列表->(@事件列表长度 - 1) * 2 = @true
;;        @完成度 = [对象]
;;        @完成度->第一章 = @false
;;        @完成度->@事件列表长度 = @true
;;;或者 @系统语言 == ja_JP + (@事件列表长度 >= 10)
;;    [样式 基础样式=@样式#对话框主体_日文]
;;;否则
;;    [提示 类型=错误 内容="当前系统语言不受支持"]
ldstr zh_CN
ldstr 系统语言
ldloc
ldloc
bne.s __cond_2__
ldstr 样式#对话框主体_简中
ldloc
ldc.i4.2
lduid 15,249,2,169,230,153,88,75,146,78,70,173,53,24,242,136
ldc.i4.1
call
pop
ldt
ldstr 初次运行
bne.s __cond_5__
ldf
ldc.i4.1
ldf
ldc.i4.0
ldc.i4.2
lduid 207,188,48,221,101,246,38,65,130,45,232,165,249,218,214,84
call
ldstr 事件列表
stloc
pop
ldt
ldc.i4.2
ldc.i4.1
ldstr 事件列表长度
sub
mul
ldstr 事件列表
pick
stloc
pop
ldc.i4.0
lduid 207,188,48,221,101,246,38,65,130,45,232,165,249,218,214,84
call
ldstr 完成度
stloc
pop
ldf
ldstr 第一章
ldstr 完成度
pick
stloc
pop
ldt
ldstr 事件列表长度
ldloc
ldstr 完成度
pick
stloc
.label __cond_5__ br.s __cond_4__
.label __cond_2__ ld10
ldstr 事件列表长度
cge
ldstr ja_JP
ldstr 系统语言
lsloc
eql
add
bf.s __cond_3__
ldstr 样式#对话框主体_日文
ldloc
ldc.i4.2
lduid 15,249,2,169,230,153,88,75,146,78,70,173,53,24,242,136
ldc.i4.1
call
br.s __cond_4__
.label __cond_3__ ldstt 5 ;当前系统语言不受支持
ldc.i4.0
ldc.i4.1
lduid 103,78,129,147,72,114,231,73,160,161,17,109,210,191,128,11
call
.label __cond_4__ pop

;;;循环 @事件列表长度 > 0
;;    @事件列表长度 -= 1
.label __loop_1__ ldc.i4.0
ldstr 事件列表长度
ldloc
ble.s __loop_1_end__
ldc.i4.1
ldstr 事件列表长度
sub
stloc
br.s __loop_1__
.label __loop_1_end__ pop

;;@en = (;lang en) + 测试1
ldstr 测试1
ldstr en
lang
add
ldstr en
stloc

;;[切换语言 目标=@en]
ldstr en
ldloc
ldc.i4.0
ldc.i4.1
lduid 135,20,238,151,59,224,59,73,153,72,89,88,29,32,85,27
call
pop

;;文件结束符
ldnul
ret