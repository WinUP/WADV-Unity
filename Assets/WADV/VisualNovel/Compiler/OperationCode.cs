// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
namespace WADV.VisualNovel.Compiler {
    /// <summary>
    /// 用于VNS的8位汇编指令集
    /// </summary>
    /// <remarks>原则上没有特别标注的指令都是原子指令，不过依据操作符和转换器的实现，C#代码也可以强行令任何与C#交互的指令失去原子性</remarks>
    public enum OperationCode : byte {
        /// <summary>
        /// 入栈32位整数0
        /// <para>格式：<code>00</code></para>
        /// </summary>
        LDC_I4_0,
        /// <summary>
        /// 入栈32位整数1
        /// <para>格式：<code>01</code></para>
        /// </summary>
        LDC_I4_1,
        /// <summary>
        /// 入栈32位整数2
        /// <para>格式：<code>02</code></para>
        /// </summary>
        LDC_I4_2,
        /// <summary>
        /// 入栈32位整数3）
        /// <para>格式：<code>03</code></para>
        /// </summary>
        LDC_I4_3,
        /// <summary>
        /// 入栈32位整数4
        /// <para>格式：<code>04</code></para>
        /// </summary>
        LDC_I4_4,
        /// <summary>
        /// 入栈32位整数5
        /// <para>格式：<code>05</code></para>
        /// </summary>
        LDC_I4_5,
        /// <summary>
        /// 入栈32位整数6
        /// <para>格式：<code>06</code></para>
        /// </summary>
        LDC_I4_6,
        /// <summary>
        /// 入栈32位整数7
        /// <para>格式：<code>07</code></para>
        /// </summary>
        LDC_I4_7,
        /// <summary>
        /// 入栈32位整数8
        /// <para>格式：<code>08</code></para>
        /// </summary>
        LDC_I4_8,
        /// <summary>
        /// 入栈32位整数
        /// <para>格式：<code>09 &lt;int32&gt;</code></para>
        /// </summary>
        LDC_I4,
        /// <summary>
        /// 入栈32位浮点数0.0
        /// <para>格式：<code>0A</code></para>
        /// </summary>
        LDC_R4_0,
        /// <summary>
        /// 入栈32位浮点数0.25
        /// <para>格式：<code>0B</code></para>
        /// </summary>
        LDC_R4_025,
        /// <summary>
        /// 入栈32位浮点数0.5
        /// <para>格式：<code>0C</code></para>
        /// </summary>
        LDC_R4_05,
        /// <summary>
        /// 入栈32位浮点数0.75
        /// <para>格式：<code>0D</code></para>
        /// </summary>
        LDC_R4_075,
        /// <summary>
        /// 入栈32位浮点数1.0
        /// <para>格式：<code>0E</code></para>
        /// </summary>
        LDC_R4_1,
        /// <summary>
        /// 入栈32位浮点数1.25
        /// <para>格式：<code>0F</code></para>
        /// </summary>
        LDC_R4_125,
        /// <summary>
        /// 入栈32位浮点数1.5
        /// <para>格式：<code>10</code></para>
        /// </summary>
        LDC_R4_15,
        /// <summary>
        /// 入栈32位浮点数1.75
        /// <para>格式：<code>11</code></para>
        /// </summary>
        LDC_R4_175,
        /// <summary>
        /// 入栈32位浮点数2.0
        /// <para>格式：<code>12</code></para>
        /// </summary>
        LDC_R4_2,
        /// <summary>
        /// 入栈32位浮点数2.25
        /// <para>格式：<code>13</code></para>
        /// </summary>
        LDC_R4_225,
        /// <summary>
        /// 入栈32位浮点数2.5
        /// <para>格式：<code>14</code></para>
        /// </summary>
        LDC_R4_25,
        /// <summary>
        /// 入栈32位浮点数2.75
        /// <para>格式：<code>15</code></para>
        /// </summary>
        LDC_R4_275,
        /// <summary>
        /// 入栈32位浮点数3.0
        /// <para>格式：<code>16</code></para>
        /// </summary>
        LDC_R4_3,
        /// <summary>
        /// 入栈32位浮点数3.25
        /// <para>格式：<code>17</code></para>
        /// </summary>
        LDC_R4_325,
        /// <summary>
        /// 入栈32位浮点数3.5
        /// <para>格式：<code>18</code></para>
        /// </summary>
        LDC_R4_35,
        /// <summary>
        /// 入栈32位浮点数3.75
        /// <para>格式：<code>19</code></para>
        /// </summary>
        LDC_R4_375,
        /// <summary>
        /// 入栈32位浮点数4.0
        /// <para>格式：<code>1A</code></para>
        /// </summary>
        LDC_R4_4,
        /// <summary>
        /// 入栈32位浮点数4.25
        /// <para>格式：<code>1B</code></para>
        /// </summary>
        LDC_R4_425,
        /// <summary>
        /// 入栈32位浮点数4.5
        /// <para>格式：<code>1C</code></para>
        /// </summary>
        LDC_R4_45,
        /// <summary>
        /// 入栈32位浮点数4.75
        /// <para>格式：<code>1D</code></para>
        /// </summary>
        LDC_R4_475,
        /// <summary>
        /// 入栈32位浮点数5.0
        /// <para>格式：<code>1E</code></para>
        /// </summary>
        LDC_R4_5,
        /// <summary>
        /// 入栈32位浮点数5.25
        /// <para>格式：<code>1F</code></para>
        /// </summary>
        LDC_R4_525,
        /// <summary>
        /// 入栈32位浮点数5.5
        /// <para>格式：<code>20</code></para>
        /// </summary>
        LDC_R4_55,
        /// <summary>
        /// 入栈32位浮点数5.75
        /// <para>格式：<code>21</code></para>
        /// </summary>
        LDC_R4_575,
        /// <summary>
        /// 入栈32位浮点数
        /// <para>格式：<code>22 &lt;float&gt;</code></para>
        /// </summary>
        LDC_R4,
        /// <summary>
        /// 入栈字符串常量（常量表引用）
        /// <para>格式：<code>23 &lt;7 bit format int32&gt;</code></para>
        /// </summary>
        LDSTR,
        /// <summary>
        /// 入栈跳转标签ID
        /// <para>格式：<code>24 &lt;7 bit format int32&gt;</code></para>
        /// </summary>
        LDENTRY,
        /// <summary>
        /// 入栈可翻译字符串（翻译表引用）
        /// <para>格式：<code>25 &lt;uint32&gt;</code></para>
        /// </summary>
        LDSTT,
        /// <summary>
        /// 入栈空值
        /// <para>格式：<code>26</code></para>
        /// </summary>
        LDNUL,
        /// <summary>
        /// 变量取值
        /// <para>格式：<code>27</code></para>
        /// <para>栈结构要求：栈顶元素描述变量名</para>
        /// </summary>
        LDLOC,
        /// <summary>
        /// 常量取值
        /// <para>格式：<code>28</code></para>
        /// <para>栈结构要求：栈顶元素描述常量名</para>
        /// </summary>
        LDCON,
        /// <summary>
        /// 入栈布尔值true
        /// <para>格式：<code>29</code></para>
        /// </summary>
        LDT,
        /// <summary>
        /// 入栈布尔值false
        /// <para>格式：<code>2A</code></para>
        /// </summary>
        LDF,
        /// <summary>
        /// 调用栈顶元素所指的插件（非原子指令）
        /// <para>格式：<code>2B</code></para>
        /// <para>栈结构要求：栈顶元素描述插件名或插件本身，第二个元素描述参数数目，之后的元素以参数名、参数值的顺序描述每个参数</para>
        /// </summary>
        CALL,
        /// <summary>
        /// 出栈栈顶元素
        /// <para>格式：<code>2C</code></para>
        /// </summary>
        POP,
        /// <summary>
        /// 生成快速对话（非原子指令）
        /// <para>格式：<code>2D</code></para>
        /// <para>栈结构要求：栈顶元素描述角色，第二个元素描述对话内容</para>
        /// </summary>
        DIALOGUE,
        /// <summary>
        /// 取出栈顶元素并放入该元素的真值
        /// <para>格式：<code>2E</code></para>
        /// <para>栈结构要求：栈不能为空</para>
        /// </summary>
        BVAL,
        /// <summary>
        /// 取出栈顶两个元素相加并放入计算结果
        /// <para>格式：<code>2F</code></para>
        /// <para>栈结构要求：栈内至少有两个元素，栈顶描述表达式右侧，第二个元素描述表达式左侧</para>
        /// </summary>
        ADD,
        /// <summary>
        /// 取出栈顶两个元素相减并放入计算结果
        /// <para>格式：<code>30</code></para>
        /// <para>栈结构要求：栈内至少有两个元素，栈顶描述表达式右侧，第二个元素描述表达式左侧</para>
        /// </summary>
        SUB,
        /// <summary>
        /// 取出栈顶两个元素相乘并放入计算结果
        /// <para>格式：<code>31</code></para>
        /// <para>栈结构要求：栈内至少有两个元素，栈顶描述表达式右侧，第二个元素描述表达式左侧</para>
        /// </summary>
        MUL,
        /// <summary>
        /// 取出栈顶两个元素相除并放入计算结果
        /// <para>格式：<code>32</code></para>
        /// <para>栈结构要求：栈内至少有两个元素，栈顶描述表达式右侧，第二个元素描述表达式左侧</para>
        /// </summary>
        DIV,
        /// <summary>
        /// 取出栈顶元素并放入该元素取反值
        /// <para>格式：<code>33</code></para>
        /// <para>栈结构要求：栈不能为空</para>
        /// </summary>
        NEG,
        /// <summary>
        /// 取出栈顶两个元素并放入其真值比较结果
        /// <para>格式：<code>34</code></para>
        /// <para>栈结构要求：栈内至少有两个元素</para>
        /// </summary>
        EQL,
        /// <summary>
        /// 取出栈顶两个元素并放入栈顶元素是否不比第二个元素小的结果
        /// <para>格式：<code>35</code></para>
        /// <para>栈结构要求：栈内至少有两个元素</para>
        /// </summary>
        CGE,
        /// <summary>
        /// 取出栈顶两个元素并放入栈顶元素是否比第二个元素大的结果
        /// <para>格式：<code>36</code></para>
        /// <para>栈结构要求：栈内至少有两个元素</para>
        /// </summary>
        CGT,
        /// <summary>
        /// 取出栈顶两个元素并放入栈顶元素是否不比第二个元素大的结果
        /// <para>格式：<code>37</code></para>
        /// <para>栈结构要求：栈内至少有两个元素</para>
        /// </summary>
        CLE,
        /// <summary>
        /// 取出栈顶两个元素并放入栈顶元素是否比第二个元素小的结果
        /// <para>格式：<code>38</code></para>
        /// <para>栈结构要求：栈内至少有两个元素</para>
        /// </summary>
        CLT,
        /// <summary>
        /// 变量赋值
        /// <para>格式：<code>39</code></para>
        /// <para>栈结构要求：栈顶元素描述变量名，第二个元素描述变量值</para>
        /// </summary>
        STLOC,
        /// <summary>
        /// 新建常量
        /// <para>格式：<code>3A</code></para>
        /// <para>栈结构要求：栈顶元素描述常量名，第二个元素描述常量值</para>
        /// </summary>
        STCON,
        /// <summary>
        /// 内存堆栈赋值
        /// <para>格式：<code>3B</code></para>
        /// <para>栈结构要求：栈顶元素描述赋值目标，第二个元素描述值</para>
        /// </summary>
        STMEM,
        /// <summary>
        /// 取子元素
        /// <para>格式：<code>3C</code></para>
        /// <para>栈结构要求：栈顶元素描述父对象，第二个元素描述子对象名称</para>
        /// </summary>
        PICK,
        /// <summary>
        /// 创建作用域
        /// <para>格式：<code>3D</code></para>
        /// </summary>
        SCOPE,
        /// <summary>
        /// 销毁作用域
        /// <para>格式：<code>3E</code></para>
        /// </summary>
        LEAVE,
        /// <summary>
        /// 返回到上一个有记录的执行偏移地址
        /// <para>格式：<code>3F</code></para>
        /// </summary>
        RET,
        /// <summary>
        /// 调用栈顶元素所指的函数（该指令是一系列变量赋值指令+BR的简写）
        /// <para>格式：<code>40</code></para>
        /// <para>栈结构要求：栈顶元素描述函数名，第二个元素描述参数数目，之后的元素以参数名、参数值的顺序描述每个参数</para>
        /// </summary>
        FUNC,
        /// <summary>
        /// 如果栈顶元素真值不为true则跳转到指定标签处
        /// <para>格式：<code>41 &lt;7 bit format int32&gt;</code></para>
        /// <para>栈结构要求：栈不能为空</para>
        /// </summary>
        BF,
        /// <summary>
        /// 无条件跳转到指定标签处
        /// <para>格式：<code>42 &lt;7 bit format int32&gt;</code></para>
        /// </summary>
        BR,
        /// <summary>
        /// 导入脚本执行结果（非原子指令）
        /// <para>格式：<code>43</code></para>
        /// <para>栈结构要求：栈顶元素描述目标脚本路径</para>
        /// </summary>
        LOAD,
        /// <summary>
        /// 导出表达式结果
        /// <para>格式：<code>44</code></para>
        /// <para>栈结构要求：栈顶元素描述导出项名，第二个元素描述项值</para>
        /// </summary>
        EXP
    }
}