# 异步编程与`async`&`await`关键字

C# 异步编程基于Task任务模型，包括异步方法 `async` `await` 关键字

## Task任务模型

在 .NET Core 中使用 Task 任务模型所需的所有对象都集中在 `System.Threading.Tasks` 命名空间中。  
最常用到的是 `Task` 类及其泛型类 `Task<TResult>`，创建任务即初始化 `Task` 类型，可以赋值给变量，通过 `Task.Run` 静态方法给信号开始执行任务代码，然后执行其他不依赖该任务的代码。

## 异步方法

定义异步方法可以在声明方法时使用 `async` 关键字声明这是一个异步方法。按照约定，异步方法以Async结尾。返回值可以选用 `void` `Task` `Task<TResult>` `IAsyncEnumerable<TResult>`  
注意，调用异步方法的那一刻就已经开始执行 `Task` 了，无需再使用 `Task.Run` 方法  
返回值为 `void` 时不能 `await`。

### 异步方法中处理异常
根据不同的返回值，C#编译器会生成不同的IL代码，且方法体内使用的返回值的类型也不同
返回值使用 `void` 的时候，则生成的异步方法在执行是如果遇到异常，则直接通过同步上下文抛出异常，这会立刻中断调用该异步方法的线程/任务  
返回值使用 `Task` 时在处理异常时有些不一样，在任务内部遇到异常之后，只有在对该任务使用 `await` 运算符时才会捕获异常

### 在异步方法中返回
返回值为 `void` `Task` 的异步方法，在方法体中使用空的 `return;` 语句返回，而剩下两种都需要 `return TResult;`  
`Task<TResult>` 是 `Task` 的简单扩展，而`IAsyncEnumerable<TResult>` 则必须搭配 `yield return` 返回。同理，如果想在修饰了 `async` 关键字的异步方法中使用 `yield return`，则返回值必须是`IAsyncEnumerable<TResult>`


## `async` 以及 `await` 关键字

`async` 及 `await` 是C#异步编程的核心，其中 `async` 定义异步方法，`await` 处理异步调用。  
`await` 关键字只能在声明了 `async` 的异步方法中使用，用来等待一个 `Task`/`ValueTask` 完成，如果有的话把该任务的返回值作为 `await` 表达式的值。  
在等待任务完成的时候会把控制流交回给调用方[^1]，调用方再去做其他不依赖该异步方法的工作。同理，异步方法在开始一个异步任务 `TaskA` 的时候，可以去做其他不依赖 `TaskA` 的工作。在需要其结果时，才使用 `await` 关键字，如果 `TaskA` 也使用了 `await` 等待其他工作，那么控制流交回 `AsyncMethod`。  

复制以下代码执行 `await Main();` 后控制台由小到大输出数字

```C#
// 为方便演示，定义了 L/P 方法用于 打印/模拟耗时任务
async Task Main()
{
    //....
    L(0);
    var result = await AsyncMethod();
    // 一般提前开始任务再在需要的时候 await 而不是直接 await
    // 而在有专门针对的框架如aspnet中无需担心阻塞，此时await会把任务直接交由框架托管，不会直接阻塞线程
    L(result);
}

async Task<string> AsyncMethod()
{
    L(1);
    var taskA = TaskA();
    L(3);
    //....
    // 不依赖 TaskA 的工作
    // 可能与 TaskA 一起执行
    //....
    await taskA; // 需要该任务完成才可继续
    L(5);
    return "done";
}

async Task TaskA()
{
    L(2);
    P();
    await Task.Delay(2000);
    L(4);
    //....
}

void L(object i)
{
    Console.Write(i);
    Console.CursorLeft++;
}

void P() => Thread.Sleep(2000);
```

控制流从 `main` 方法开始，执行到异步调用，控制流进入 `AsyncMethod`，马上又会遇到一个异步方法调用，控制流再进入 `TaskA`，在TaskA遇到任何 `await` 运算符之前，这个方法是同步运行的，会阻塞调用方，遇到 `await` 之后方法会马上返回一个 `Task` 任务给调用方，即 `TaskA` 转入异步执行了  
此时 `AsyncMethod` 会继续执行后续代码，需要等待 `TaskA` 的时候再 `await`  

整理一下流程，控制流从 `main` 到 `AsyncMethod` 然后开始 `TaskA`，此时如果 `TaskA` 执行到 `await`，那么控制流会返回到 `AsyncMethod` 执行不依赖 `TaskA`
的工作  
`TaskA` 遇到 `await` 后就转入异步执行，除非在调用方 `await` 返回的 `Task` 对象将不再阻塞 `AsyncMethod`。  

在异步方法中，`await` 之前的代码都是同步执行的，与调用同步方法别无二致，使用 `await` 之后转入异步调用，即马上返回 `Task` 给调用方，调用方再执行其它任务以尽可能利用更多资源  

### 细琐 `async` `await` 关键字

`async` `await` 都只是语法糖，使用的时候C#编译器会额外生成代码，用户可以写出完全不使用 `async` `await` 关键字而事实上异步的方法，事实上在引入这组关键字之前人们就已经在编写异步代码了  
具体操作是这样的，在原本异步方法需要使用 `await` 的地方直接 `new` 一个 `Task` 并 `return`，而返回的 `Task` 要执行的代码则是原本异步方法 `await` 的表达式及后续的所有代码  
实际上编译器也是这样做的，修饰了 `async` 关键字的方法会告诉编译器，这个方法在遇到 `await` 关键字之后需要作特殊处理  

以下展示了异步方法如果不用关键字的一般写法，省略了 L/P 方法定义  

```C#
// 以下 L/P 方法仅为方便演示，实际作不存在处理
void Main() {
    //....
    L(0);
    var result = AsyncMethod().Result;
    // 一般提前开始任务再在需要的时候查看结果，任务未完成时调用会阻塞线程
    L(result);
}

Task<string> AsyncMethod()
{
    L(1);
    var taskA = TaskA();
    L(3);
    //....
    // 不依赖 TaskA 的工作
    // 可能与 TaskA 一起执行
    //....
    return Task.Run(() =>
    {
        L(4);
        taskA.Wait();
        L(6);
        return "done";
    });
}

Task TaskA()
{
    L(2);
    P();
    return Task.Run(() =>
    {
        P(); // 如果注释掉这一句，那么 4.5 大概率先于 4 打印
             // 因为从这里开始已经转入异步调用了，其他异步点同理
        L(4.5);
        //....
        // 这里的代码事实上异步执行
        //....
        P();
        L(5);
    });
}
```

[^1]: "即一般认为的从方法中返回"
