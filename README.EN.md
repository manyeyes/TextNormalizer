（ [简体中文](README.md) |  English ）

# TextNormalizer

An open-source .NET library for **standardizing English text and numbers**. It automatically handles issues like number conversion, abbreviation expansion, and format unification in unstructured English content, making it suitable for scenarios such as data cleaning and NLP preprocessing.


## 📸 Visual Example of Results
Below are the core functionality results of the library (input text on the left, normalized output on the right), covering common text issues in real-world scenarios:

### 1. Number Normalization (Text → Numerals + Standard Format)
| Input Text | Normalized Output |
|------------|-------------------|
| "five twenty five" | "525" |
| "eight of these products had sales exceeding 100 million yuan, three euros and sixty five cents, on august twenty sixth twenty twenty one" | "8 of these products had sales exceeding 100000000 yuan, €3.65, on august 26th 2021" |
| "revenue of 5.8 billion yuan, average sales 549,000 yuan" | "revenue of 5800000000 yuan, average sales 549000 yuan" |


### 2. Text Abbreviation & Title Expansion
| Input Text | Normalized Output |
|------------|-------------------|
| "Mr. Park visited Assoc. Prof. Kim Jr." | "mister park visited associate professor kim junior" |
| "Chagee's founder said in the first quarter" | "chagee is founder said in the 1st quarter" |


### 3. Punctuation & Format Cleaning
| Input Text | Normalized Output |
|------------|-------------------|
| "100 million yuan,,,, [$14.1 million yuan],,," | "100000000 yuan, $14100000 yuan" |
| "sales over 700 million cups ( three years ago )" | "sales over 700000000 cups 3 years ago" |


## 🚀 Quick Start in 3 Steps

### 1. Prerequisites
Supported .NET frameworks:
- .NET Framework 4.8
- .NET Standard 2.0+
- .NET 6.0+ (compatible via .NET Standard)


### 2. Installation
#### Option 1: Install via NuGet (Recommended)
```bash
# .NET CLI
dotnet add package TextNormalizer

# Package Manager Console
Install-Package TextNormalizer
```

#### Option 2: Reference Source Code
1. Clone the repository: `git clone https://github.com/manyeyes/TextNormalizer.git`
2. Add the `TextNormalizer` project to your solution and reference it.


### 3. Core Code Example
Complete normalization can be achieved with just 3 lines of code:
```csharp
using TextNormalizer;

// 1. Create normalizer instances
var textNormalizer = new EnglishTextNormalizer();
var spellingNormalizer = new EnglishSpellingNormalizer();

// 2. Input text to be processed
string input = "Mr. Park said sales exceeded three million yuan on july fifth twenty twenty three.";

// 3. Perform normalization (text + spelling processing)
string normalizedText = textNormalizer.GetEnglishTextNormalizer(input);
normalizedText = spellingNormalizer.GetEnglishSpellingNormalizer(normalizedText);

// Output result: "mister park said sales exceeded 3000000 yuan on july 5th 2023."
Console.WriteLine(normalizedText);
```


## 🔧 Core Capabilities
| Capability Category | Specific Features | Examples |
|---------------------|-------------------|----------|
| Number Processing   | Text-based numbers → Arabic numerals | "twenty nineteen" → "2019" |
|                     | Currency unit standardization | "100 million dollars" → "$100000000" |
|                     | Date format unification | "august twenty sixth" → "august 26th" |
|                     | Percentage/symbol conversion | "ninety percent" → "90%" |
| Text Processing     | Title/abbreviation expansion | "Assoc. Prof." → "associate professor" |
|                     | Possessive/contraction parsing | "He's" → "he is" |
|                     | Redundant punctuation cleaning | "yuan,,,, " → "yuan " |
|                     | Case unification | "By the end" → "by the end" |
| Spelling Processing | British → American spelling unification | "mobilisation" → "mobilization" |


## ✅ Test Validation
The project includes built-in unit tests covering all core scenarios to ensure stability:
- **Number Normalization**: Validates 30+ scenarios (including currency, dates, and special formats like "double zero seven"→"007")
- **Text Processing**: Covers common abbreviations (Mr./Prof./Jr.) and contractions (Let's/He's)
- **Format Compatibility**: Handles text with interfering characters like parentheses and commas

To run tests:
```bash
dotnet test TextNormalizer.Tests.csproj
```