using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;

namespace Katren.DiReg
{
    /// <summary>
    /// Жизненный цикл регистрируемого сервиса
    /// </summary>
    public enum DiLifetime
    {
        /// <summary>
        ///  Синглтон
        /// </summary>
        Singleton,

        /// <summary>
        ///  Синглтон в рамках скоупа. Например, на каждый обрабатываемый входящий запрос в AspNet
        /// </summary>
        Scoped,

        /// <summary>
        ///  На каждый запрос - свой инстанс
        /// </summary>
        Transient
    }

    /// <summary>
    /// Атрибут для пометки на автоматическую регистрацию класса в Dependency Injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class DiRegAttribute : Attribute
    {
        /// <summary>
        /// Интерфейс для регистрации в DI
        /// </summary>
        public Type InterfaceType { get; }

        /// <summary>
        /// Тип жизненного цикла для красса в DI
        /// </summary>
        public DiLifetime LifeTime { get; }

        /// <summary>
        /// Конструктор для регистрации в DI класса по интерфейсу
        /// </summary>
        /// <param name="interfaceType">Интерфейс для регистрации в DI</param>
        /// <param name="lifeTime">Жизненный цикл для регистрации в DI</param>
        public DiRegAttribute(Type interfaceType, DiLifetime lifeTime = DiLifetime.Transient)
        {
            InterfaceType = interfaceType;
            LifeTime = lifeTime;
        }

        /// <summary>
        /// Конструктор для регистрации в DI класса без интерфейса
        /// </summary>
        /// <param name="lifeTime">Жизненный цикл для регистрации в DI</param>
        public DiRegAttribute(DiLifetime lifeTime = DiLifetime.Transient)
        {
            InterfaceType = null;
            LifeTime = lifeTime;
        }
    }

    /// <summary>
    /// Утилитарные методы для работы с AutoRegAttribute
    /// </summary>
    public static class AutoRegUtils
    {
        /// <summary>
        /// Метод для автоматической регистрации в DI всех классов, помеченных атрибутом AutoReg из указанных сборок
        /// </summary>
        /// <param name="services">Коллекция сервисов, в которую будут зарегистрированны классы помеченные атрибутом AutoReg</param>
        /// <param name="assemblies">Список сборок, из которых будут получены классы для регистрации в Dependency Injection</param>
        public static void AddDiRegClasses(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            Contract.Requires(services != null);
            Contract.Requires(assemblies != null);

            IEnumerable<Type> typeList = assemblies.SelectMany(s => s.GetTypes()).Where(t => t.IsClass);
            foreach (Type serviceType in typeList)
            {
                foreach (object attrib in serviceType.GetCustomAttributes())
                {
                    if (!(attrib is DiRegAttribute attr))
                        continue;

                    Type interfaceType = attr.InterfaceType ?? serviceType;
                    switch (attr.LifeTime)
                    {
                        case DiLifetime.Singleton:
                            services.AddSingleton(interfaceType, serviceType);
                            break;

                        case DiLifetime.Scoped:
                            services.AddScoped(interfaceType, serviceType);
                            break;

                        case DiLifetime.Transient:
                            services.AddTransient(interfaceType, serviceType);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Метод для автоматической регистрации в DI всех классов, помеченных атрибутом AutoReg в указанной сборке
        /// </summary>
        /// <param name="services">Коллекция сервисов, в которую будут зарегистрированны классы помеченные атрибутом AutoReg</param>
        /// <param name="assembly">Сборка из которой будут получены классы для регистрации в Dependency Injection</param>
        public static void AddDiRegClasses(this IServiceCollection services, Assembly assembly)
        {
            AddDiRegClasses(services, new [] {assembly});
        }
    }
}