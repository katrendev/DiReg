# DiReg

## О проекте

Репозиторий содержит маленький атрибут для .net core 2.2 приложений, позволяющий регистрировать классы в MS Dependency Injection.  
Собирается в NuGet пакет.

- [Установка](#Установка)
- [Использование](#Использование)

## Установка

```bash
dotnet add package Katren.DiReg
```

## Использование

После успешной установки пакета для регистрации всех классов с атрибутом DiReg достаточно вызывать метод расшинения IServiceCollection.**AddDiRegClasses()**.

Для регистрации вашего класса в DI достаточно указать ему атрибут DiReg.  
Возможны 2 варианта использования:  

### 1. Простая регистрация, когда тип сервиса совпадает с типом имплементации такового

```csharp
[DiReg(DiLifetime: lifeTime)]
public class SimpleRegClass {}
```

Эквивалентно вызову:

```csharp
IServiceCollection.Add***(SimpleRegClass, SimpleRegClass);
```

### 2. Регистрация сервиса, когда тип сервиса не совпадает с типом имплементации такового

```csharp
public interface ISomeService {}

[DiReg(typeof(ISomeService), DiLifetime: lifeTime)]
public class SomeService {}
```

Эквивалентно вызову:

```csharp
IServiceCollection.Add***(ISomeService, SomeService);
```

--------
В любом случае, неоходимо всегда указывать тип жизненного цикла регистрируемого сервиса.  
Делается это с помощью enum DiLifetime.  
Поддерживается 3 варианта жизненного цикла:  

- **DiLifetime.Singleton** - Синглтон. Эквивалентно вызову метода:

    ```csharp
    IServiceCollection.AddSingleton(***);
    ```

- **DiLifetime.Scoped** - Создаётся ровно 1 экземпляр в рамках скоупа. Например, для AspNet один экземпляр на обработку запроса. Эквивалентно вызову метода:

    ```csharp
    IServiceCollection.AddScoped(***);
    ```

- **DiLifetime.Transient** - Создавать экземпляр класса на каждый запрос. Эквивалентно вызову метода:

    ```csharp
    IServiceCollection.AddTransient(***);
    ```
