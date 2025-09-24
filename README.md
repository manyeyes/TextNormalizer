（ 简体中文  | [英文](README.EN.md) ）

# TextNormalizer

一个用于**标准化英文文本与数字**的开源 .NET 库，可自动处理非结构化英文内容中的数字转译、缩写展开、格式统一等问题，适用于数据清洗、NLP 预处理等场景。


## 📸 直观效果示例
以下是库的核心功能运行结果（左为输入文本，右为归一化后输出），覆盖真实场景中常见的文本问题：

### 1. 数字归一化（文字→数字+标准格式）
| 输入文本 | 归一化输出 |
|----------|------------|
| "five twenty five" | "525" |
| "eight of these products had sales exceeding 100 million yuan, three euros and sixty five cents, on august twenty sixth twenty twenty one" | "8 of these products had sales exceeding 100000000 yuan, €3.65, on august 26th 2021" |
| "revenue of 5.8 billion yuan, average sales 549,000 yuan" | "revenue of 5800000000 yuan, average sales 549000 yuan" |


### 2. 文本缩写与头衔展开
| 输入文本 | 归一化输出 |
|----------|------------|
| "Mr. Park visited Assoc. Prof. Kim Jr." | "mister park visited associate professor kim junior" |
| "Chagee's founder said in the first quarter" | "chagee is founder said in the 1st quarter" |


### 3. 标点与格式清理
| 输入文本 | 归一化输出 |
|----------|------------|
| "100 million yuan,,,, [$14.1 million yuan],,," | "100000000 yuan, $14100000 yuan" |
| "sales over 700 million cups ( three years ago )" | "sales over 700000000 cups 3 years ago" |


## 🚀 3步快速使用

### 1. 前置要求
支持的 .NET 框架：
- .NET Framework 4.8
- .NET Standard 2.0+
- .NET 6.0+（兼容 .NET Standard）


### 2. 安装
#### 方式1：NuGet 安装（推荐）
```bash
# .NET CLI
dotnet add package TextNormalizer

# Package Manager Console
Install-Package TextNormalizer
```

#### 方式2：源码引用
1. 克隆仓库：`git clone https://github.com/manyeyes/TextNormalizer.git`
2. 将 `TextNormalizer` 项目添加到你的解决方案并引用。


### 3. 核心代码示例
只需3行代码即可实现完整归一化：
```csharp
using TextNormalizer;

// 1. 创建归一化器实例
var textNormalizer = new EnglishTextNormalizer();
var spellingNormalizer = new EnglishSpellingNormalizer();

// 2. 输入待处理文本
string input = "Mr. Park said sales exceeded three million yuan on july fifth twenty twenty three.";

// 3. 执行归一化（文本+拼写双处理）
string normalizedText = textNormalizer.GetEnglishTextNormalizer(input);
normalizedText = spellingNormalizer.GetEnglishSpellingNormalizer(normalizedText);

// 输出结果："mister park said sales exceeded 3000000 yuan on july 5th 2023."
Console.WriteLine(normalizedText);
```


## 🔧 核心能力
| 能力分类 | 具体功能 | 示例 |
|----------|----------|------|
| 数字处理 | 文字数字→阿拉伯数字 | "twenty nineteen" → "2019" |
|          | 金额单位标准化 | "100 million dollars" → "$100000000" |
|          | 日期格式统一 | "august twenty sixth" → "august 26th" |
|          | 百分比/符号转换 | "ninety percent" → "90%" |
| 文本处理 | 头衔/缩写展开 | "Assoc. Prof." → "associate professor" |
|          | 所有格/缩写解析 | "He's" → "he is" |
|          | 标点冗余清理 | "yuan,,,, " → "yuan " |
|          | 大小写统一 | "By the end" → "by the end" |
| 拼写处理 | 英美拼写统一 | "mobilisation" → "mobilization" |


## ✅ 测试验证
项目内置单元测试覆盖所有核心场景，确保稳定性：
- **数字归一化**：验证30+场景（含金额、日期、特殊格式如 "double zero seven"→"007"）
- **文本处理**：覆盖常见缩写（Mr./Prof./Jr.）、 contractions（Let's/He's）
- **格式兼容**：处理带括号、逗号等干扰符的文本

运行测试：
```bash
dotnet test TextNormalizer.Tests.csproj
```