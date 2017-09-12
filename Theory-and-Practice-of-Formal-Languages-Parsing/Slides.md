<!-- $theme: default -->
<!-- page_number: true -->

# Теория и практика формальных языков

* Иван Кочуркин
* Работаю в [Positive Technologies](https://www.ptsecurity.com/ru-ru/) над открытым универсальным сигнатурным анализатором кода [PT.PM](https://github.com/PositiveTechnologies/PT.PM)
* Подрабатываю в [Swiftify](http://swiftify.io/), веб-сервисе для конвертинга
  кода Objective-C в Swift
* Веду активную деятельность на [GitHub](https://github.com/KvanTTT)
* Пишу статьи на [Хабре](https://habrahabr.ru/users/kvanttt/) и [GitHub](https://github.com/KvanTTT/Articles) под ником KvanTTT

---

# Почему не Regex?

1. `<table>(.*?)</table>`
2. А если аттрибуты? `<table.*?>(.*?)</table>`
3. А если элементы? `tr`, `td`
4. А если комментарии? `<!-- html comment -->`
5. ...
6. [NO NOO̼O​O NΘ stop the an​*̶͑̾̾​̅ͫ͏̙̤g͇̫͛͆̾ͫ̑͆l͖͉̗̩̳̟̍ͫͥͨe̠̅s ͎a̧͈͖r̽̾̈́͒͑e n​ot rè̑ͧ̌aͨl̘̝̙̃ͤ͂̾̆ ZA̡͊͠͝LGΌ ISͮ̂҉̯͈͕̹̘̱ TO͇̹̺ͅƝ̴ȳ̳ TH̘Ë͖́̉ ͠P̯͍̭O̚​N̐Y̡ H̸̡̪̯ͨ͊̽̅̾̎Ȩ̬̩̾͛ͪ̈́̀́͘ ̶̧̨̱̹̭̯ͧ̾ͬC̷̙̲̝͖ͭ̏ͥͮ͟Oͮ͏̮̪̝͍M̲̖͊̒ͪͩͬ̚̚͜Ȇ̴̟̟͙̞ͩ͌͝S̨̥̫͎̭ͯ̿̔̀ͅ](https://stackoverflow.com/a/1732454/1046374)

---

# Лексемы и токены

* **Лексема** - распознанная последовательность символов
* **Токен** - лексема + тип

```ANTLR
MyKeyword: 'var';
Id:        [a-z]+;
Digit:     [0-9]+;
Comment:   '<!--' .*? '-->';
```

* Пример:
`TagOpen(<) Identifier(html) TagClose(>) Whitespace()`

---

# Дерево разбора и AST

<!-- Возможно все же заменить на свою картинку -->

* **Дерево разбора** - древовидная структура, распознанная из потока токенов
* **AST** - дерево разбора без пробелов, точек с запятых и т.д.

![Лексер и парсер](https://habrastorage.org/files/6c4/385/fbe/6c4385fbe3d8471982c9b2a030106d38.png)

---

# Типы парсеров

* Готовые библиотеки парсинга (regex, JSON.NET)
  * API
  * Только самые распространенные языки
* Парсеры, написанные вручную (Roslyn)
  * Большие возможности и гибкость
  * Большой порог вхождения
  * Медленная скорость разработки
* Автоматически сгенерированные парсеры (ANTLR)
  * Порог вхождения
  * Быстрая скорость разработки после освоения
  * Меньшая гибкость по сравнению с ручными парсерами

---

# Грамматика

Формальное описание языка, которое может быть использовано для распознавания его структуры.

### Пример грамматики

```ANTLR
expr
    : expr '*' expr
    | expr '+' expr
    | ID '(' args ')'
    | ID
    ;

ID: [a-zA-Z]+;
```

### Пример данных

```
a + b * c
```

---

# Типы языков

## Иерархия Хомского

* Регулярные
* Контекстно-свободные
* Контекстно-зависимые
* Тьюринг-полные

Пример КС-КЗ конструкции: `T a = new T()`

---

# Инструменты и библиотеки под C#

* Генераторы
  * Контекстно-свободные (ANTLR, Coco/R, Gardens Point Parser Generator, Grammatica, Hime Parser Generator, LLLPG)
  * Безлексерные PEG (IronMeta, Pagarus)
* Комбинаторы (Parseq, Parsley, LanguageExt.Parsec, Sprache, Superpower)
* Языковые фреймворки (JetBrains MPS, Nitra, Roslyn) 

Детальное описание: [Parsing In C#: Tools And Libraries](https://tomassetti.me/parsing-in-csharp).

---

# Парсер-комбинаторы

* Использование внутри языка разработки (C#)
* Использование в IDE

Библиотеки:

* [Sprache](https://github.com/sprache/Sprache)
* [Superpower](https://github.com/datalust/superpower)

---

# Примеры кода парсер-комбинатора

```CSharp
// Parse any number of capital 'A's in a row
var parseA = Parse.Char('A').AtLeastOnce();
```

Правило для `id`:

```CSharp
Parser<string> identifier =
    from leading in Parse.WhiteSpace.Many()
    from first in Parse.Letter.Once()
    from rest in Parse.LetterOrDigit.Many()
    from trailing in Parse.WhiteSpace.Many()
    select new string(first.Concat(rest).ToArray());

var id = identifier.Parse(" abc123  ");

Assert.AreEqual("abc123", id);
```

---

# Проблемы и задачи парсинга

На основе ANTLR и Roslyn.

* Неоднозначность
* Контекстно-зависимые конструкции
* Регистронезависимость
* Островные языки и конструкции
* Скрытые токены
* Препроцессорные директивы
* Парсинг фрагментов кода
* Обработка и восстановление от ошибок

---

# Неоднозначность

Пример: `var var = 100500;`

### Решение с помощью грамматики

```ANTLR
// Lexer
VAR: 'var';
ID:  [0-9a-zA-Z];

// Parser
varDeclaration
    : VAR identifier ('=' expression)? ';'
    ;

identifier
    : ID
    // Other conflicted tokens
    | VAR;
```

---

# Объектный конструктор в C#

```CSharp
class Foo
{
    public string Bar { get; set; }
}
public string Bar { get; set; } = "Bar2";

...

foo = new Foo
{
    Bar = Bar // Umbiguity here
};
```

---

# Какой результат возвращает `nameof`?

```CSharp
class Foo
{
    public string Bar { get; set; }
}

static void Main(string[] args)
{
    var foo = new Foo();
    WriteLine(nameof(foo.Bar));
}
```

---

# `nameof` как функция и оператор

```CSharp
class Foo
{
    public string Bar { get; set; }
}

static void Main(string[] args)
{
    var foo = new Foo();
    WriteLine(nameof(foo.Bar));
}

static string nameof(string str)
{
    return "42";
}
```

<!-- ![Troll-Face](Troll-Face.png) -->

---

# Неоднозначность: решение с использованием вставок кода

* **Действия** - производят вычисления на целевом языке парсера.
* **Семантические предикаты** - возвращают результат.

```ANTLR
// Lexer
ID:  [0-9a-zA-Z];

// Parser
varDeclaration
    : id {_input.Lt(-1).Text == "var"}? id ('=' expression)? ';'
    ;

id
    : ID;
```

---

# Контекстно-зависимые конструкции

[Heredoc](http://php.net/manual/en/language.types.string.php) в PHP или интерполируемые строки в C#

```PHP
<?php
    echo <<< HeredocIdentifier
Line 1.
Line 2.
HeredocIdentifier
;
```

### Решение

Использование вставок кода, смотри лексер [PHP](https://github.com/antlr/grammars-v4/blob/master/php/PHPLexer.g4).

---

# `$"Интерполируемые строки в C# {2+2*2}"`

```CSharp
WriteLine($"{p.Name} is \"{p.Age} year{(p.Age == 1 ? "" : "s")} old");
WriteLine($"{(p.Age == 2 ? $"{new Person { } }" : "")}");
WriteLine($@"\{p.Name}
           ""\");
WriteLine($"Color[R={func(b: 3):#0.##}, G={g:#0.##}, B={b:#0.##}]");

```

Реализация в лексере [C#](https://github.com/antlr/grammars-v4/blob/master/csharp/CSharpLexer.g4).

---

# Регистронезависимость

Языки: Delphi, T-SQL, PL/SQL и другие.

### Решение с помощью грамматики

**Фрагментный токен** облегчает запись других токенов.

```ANTLR
Abstract:           A B S T R A C T;
BoolType:           B O O L E A N | B O O L;
BooleanConstant:    T R U E | F A L S E;

fragment A: [aA];
fragment B: [bB];
```

Без использования фрагметных токенов:

```ANTLR
Abstract:           [Aa][Bb][Ss][Tt][Rr][Aa][Cc][Tt];
```

---

# Регистронезависимость: решение на уровне рантайма

```ANTLR
Abstract:           'ABSTRACT';
BoolType:           'BOOLEAN' | 'BOOL';
BooleanConstant:    'TRUE' | 'FALSE';
```

Используется [CaseInsensitiveInputStream](https://gist.github.com/sharwell/9424666).
Чувствительные к регистру токены обрабатываются на этапе обхода дерева. Например `$id` и `$ID` в PHP.

Достоинства:

* Код лексера чище и проще
* Производительность выше

---

# Островные языки и конструкции

<center>
<img src="Islands.jpg " style="width: 500px;"/>
</center>
<br>

JavaScript внутри PHP или C# внутри Aspx.

```PHP
<?php
<head>
  <script type="text/javascript">
    document.body.innerHTML="<svg/onload=alert(1)>"
  </script>
</head>
```

---

# Островные языки и конструкции

* Использовать режимы переключения лексем `mode`.
* Сначала парсинг **PHP**. Текст внутри тегов `<script>` - обычная строка.
* Затем парсинг **JavaScript** во время обхода дерева.

---

# PHP: альтернативный синтаксис

Смесь блоков кода на HTML и PHP

```PHP
<?php switch($a): case 1: // without semicolon?>
        <br>
    <?php break ?>
    <?php case 2: ?>
        <br>
    <?php break;?>
    <?php case 3: ?>
        <br>
    <?php break;?>
<?php endswitch; ?>
```

---

# Использование отдельного режима лексем для JavaScript

```ANTLR
//SCRIPT_BODY: .*? '</script>'; // "Жадная" версия
SCRIPT_BODY:   ~[<]+;
SCRIPT_CLOSE:  '</script>' -> popMode;
SCRIPT_DUMMY:  '<' -> type(SCRIPT_BODY);
```

* `pushMode` - зайти в другой режим распознавания лексем (JavaScript -> PHP)
* `popMode` - выйти из режима (PHP -> JavaScript)
* `type` - изменить тип токена
* `channel` - поместить токен в изолированный канал (пробелы, комментарии)

---

## Обработка `/*комментариев*/` и пр·б·лов

* Включение скрытых токенов в грамматику
  ```ANTLR
  declaration:
      property COMMENT* COLON COMMENT* expr COMMENT* prio?;
  ```
* Связывание скрытые токенов с правилами грамматики (ANTLR, Swiftify)
* Связывание скрытых токенов с основными (Roslyn)



---

# Связывание скрытых токенов с узлами дерева (Swiftify)

### Предшествующие (**Precending**)

```C
//First comment
'{' /*Precending1*/ a = b; /*Precending2*/ b = c; '}'
```

### Последующие (**Following**)

```C
'{' a = b; b = c; /*Following*/ '}' /*Last comment*/
```

### Токены-сироты (**Orphans**)

```C
'{' /*Orphan*/ '}'
```

---

# Связывание скрытых токенов со значимыми (Roslyn)

## Типы узлов Roslyn

* **Node** - не конечный узел дерева, содержащий детей
* **Token** - конечный узел (keyword, id, литерал, пунктуация)
* **Trivia** - скрытый токен без родителя, связывается с `Token`.
  * Лидирующие (**Leading**)
  * Замыкающие (**Trailing**)

```CSharp
// leading 1 (var)
// leading 2 (var)
var foo = 42; /*trailing (;)*/ int bar = 100500; //trailing (;)

// leading (EOF)
EOF
```

---

# Препроцессорные директивы (Roslyn)

```CSharp
bool trueFlag =
#if NETCORE
    true
#else
    new Random().Next(100) > 95 ? true : false
#endif
;
```

### Лидирующие для `true`

```CSharp
#if NETCORE
····
```

### Лидирующие для `;`

```CSharp
#else
····new Random().Next(100) > 95 ? true : false
#endif
```

---

# Препроцессорные директивы: одноэтапная обработка (Swiftify)

* Одновременный парсинг директив и основного языка.
* Каналы для изоляции токенов препроцессорных директив.

Интерпретация и обработка макросов вместе с функциями:

### Objective-C

```Objective-C
#define DEGREES_TO_RADIANS(degrees) (M_PI * (degrees) / 180)
```

### Swift

```Swift
func DEGREES_TO_RADIANS(degrees: Double)
   -> Double { return (.pi * degrees)/180; }
```

Пример на [Swiftify](http://objc.to/wppvxj).

---

# Препроцессорные директивы: двухэтапная обработка (Codebeat)

```CSharp
bool·trueFlag·=
·········
····true
·····
··············································
······
;
```

1. Токенизация и разбор кода препроцессорных диреткив.
2. Вычисление условных директив `#if` и компилируемых блоков кода.
3. Замена директив из исходника на пробелы.
4. Токенизация и парсинг результирующего текста.

---

# Парсинг фрагментов кода (Swiftify)

### Задача: определение корректного правила для фрагмента кода

### Применение

* Конвертинг кода в плагине IDE (в Swiftify для плагинов к AppCode, XCode)
* Определение языка программирования (в PT.PM для редактора [Approof](https://approof.ptsecurity.ru/))

### Решение

* Регулярные выражения
* Токенизация и операции с токенами
* Парсинг разными правилами

**Примеры**: [утверждения](http://objc.to/6mpmhq), [декларации методов](http://objc.to/nt25a1), [свойства](http://objc.to/vnpasw).

---

# ~~А~~Ошибки парсинга

![Errors](Errors.png)

#### Лексическая ошибка

```CSharp
class # T { }
```

Отдельный канал: `ERROR: . -> channel(ErrorChannel)`

---

# Ошибки парсинга

#### Отсутствующий и лишний токены

```CSharp
class T { // Отсутствующий токен
class T ; { } // Лишний токен
```


#### Несколько лишних токенов (режим «паники»)

```CSharp
class T { a b c }
```

<img src="Panic.png " style="width: 200px;"/>

#### Отсутствующее альтернативное подправило

---

# Ошибки парсинга в Roslyn

```CSharp
namespace App
{
    class Program
    {
        static void void Main(string[] args)  // Invalid token
        {
            a                // ';' expected
            string s = """;  // Newline in constant
            char c = '';     // Empty character literal
        }
    }
}
```

---

# Уязвимость goto fail

```C
hashOut.data = hashes + SSL_MD5_DIGEST_LEN;
hashOut.length = SSL_SHA1_DIGEST_LEN;
if ((err = SSLFreeBuffer(&hashCtx)) != 0)
    goto fail;
// ...
if ((err = SSLHashSHA1.update(&hashCtx, &signedParams)) != 0)
    goto fail;
    goto fail;  /* MISTAKE! THIS LINE SHOULD NOT BE HERE */
```

Способы выявления:

* Анализ достоверного дерева разбора (full fidelity)
* Анализ графа потока управления (CFG)

---

# Заключение о парсинге

О чем не будет рассказываться:

* Форматирование кода (антипарсинг)
* Автокомплит
* Производительность

---

# Обработка древовидных структур

* Методы обхода
* Архитектура и реализации Visitor и Listener
* Фичи C# 7 на практике
* Оптимизации

<center>
<img src="Tree.png " />
</center>

---

# Методы обхода деревьев

## Посетитель (**Visitor**)

<img align="left" src="Visitor.png" alt="" width="220px" />

* Тип возвращаемого значения для каждого правила. Например, `string` для конвертера исходных кодов (Swiftify).
* Выборочный обход дочерних узлов.

## Слушатель (**Listener**)

<img align="left" src="Listener.jpg" alt="" width="220px" />

* Посещает все узлы.
* Ограниченный функционал: можно использовать для подсчета простых метрик кода.

---

# Visitor & Listener

![Visitor & Listener](https://habrastorage.org/files/bd1/69c/535/bd169c535e854f9681520f520d0db9c3.png)

---

# Реализации Visitor

## Написанные вручную

* Долгая и утомительная разработка

## Сгенерированные (ANTLR)

* Избыточность и нарушение Code Style
* Доступны не всегда
* Универсальность (Java, C#, Python2|3, JS, C++, Go, Swift)
* Скорость разработки

## Динамические

* Медленная скорость, но хорошо для прототипов

---

# Диспетчеризация с использованием рефлексии

```CSharp
// invocation in VisitChildren
Visit((dynamic)customNode);

// visit methods

public virtual T Visit(ExpressionStatement expressionStatement)
{
    return VisitChildren(expressionStatement);
}

public virtual T Visit(StringLiteral stringLiteral)
{
    return VisitChildren(stringLiteral);
}
```

---

# Архитектура Visitor

## Архитектура

* Несколько маленьких. Создание визиторов при необходимости.
* Один большой с использованием `partial` классов.

## Формализация

Перегрузка всех методов и использование "заглушек"

```CSharp
throw new ShouldNotBeVisitedException(context);
```

---

# C\# 7: локальные функции

```CSharp
public static List<Terminal> GetLeafs(this Rule node)
{
    var result = new List<TerminalNode>();
    GetLeafs(node, result);

    // Local function
    void GetLeafs(Rule localNode, List<Terminal> localResult)
    {
        for (int i = 0; i < localNode.ChildCount; ++i)
        {
            IParseTree child = localNode.GetChild(i);
            // Is expression
            if (child is TerminalNode typedChild)
                localResult.Add(typedChild);
            GetLeafs(child, localResult);
        }
    }
    return result;
}
```

---

# Оптимизации

## Мемоизация

* Поиск первого потомка определенного типа `FirstDescendantOfType`.
* Хранение всех потомков для каждого узла дерева вместо их поиска.
* Увеличение производительности в 2-3 раза.
* Увеличение потребления памяти в 3 раза.

## Уменьшение аллокаций

* Метод `Visit` - базовый, он часто вызывается.

---

# Ресурсы

<center>
<img src="Resources.png" width="450px" />
</center>

* Исходники [презентации](https://github.com/KvanTTT/Presentations) и [примеров](https://github.com/KvanTTT/Samples).
* Статьи:
  * [Теория и практика парсинга исходников с помощью ANTLR и Roslyn](https://habrahabr.ru/company/pt/blog/210772)
  * [Обработка препроцессорных директив в Objective-C](https://habrahabr.ru/post/318954/)
* Движок поиска по шаблонам [PT.PM](https://github.com/PositiveTechnologies/PT.PM) и грамматики [grammars-v4](https://github.com/antlr/grammars-v4) ([PL/SQL](https://github.com/antlr/grammars-v4/tree/master/plsql), [T-SQL](https://github.com/antlr/grammars-v4/tree/master/tsql), [PHP](https://github.com/antlr/grammars-v4/tree/master/php), [C#](https://github.com/antlr/grammars-v4/tree/master/csharp), [Java8](https://github.com/antlr/grammars-v4/tree/master/java8-pt), [Objective-C](https://github.com/antlr/grammars-v4/tree/master/objc)).

---

# Выводы

* Парсинг - это не так просто как кажется
* Существуют языки с различной выразительностью и синтаксисом
* На C\# можно пользоваться разными методами и библиотеками парсинга
* ANTLR предоставляет широкие возможности по разработке парсеров
* Roslyn очень продуман в построении дерева, но только C# и VB
* Деревья можно обрабатывать разными способами

---

# Вопросы?

<br>

[<img align="left" src="PT.png" alt="" />](https://ptsecurity.ru/)

<br>

**[Positive Technologies на GitHub](https://github.com/PositiveTechnologies)**

<br>
<br>

[<img align="left" src="Swiftify.png" alt="" />](http://swiftify.com/)

<br>

**[Swiftify](https://objectivec2swift.com/)**
