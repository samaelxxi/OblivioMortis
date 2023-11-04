using System;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using Services;


public class ServiceLocator : MonoBehaviour
{
    private static readonly Dictionary<Type, object> Services = new();


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    public static void Initialize()
    {
        foreach (var serviceType in ReflectionService.GetAllAutoRegisteredServices())
        {
            Debug.Log($"Registering {serviceType.Name}");
            if (IsRegistered(serviceType)) continue;
            
            if (serviceType.IsMonoBehaviour())
            {
                FindOrCreateMonoService(serviceType);
            }
            else
            {
                RegisterNewInstance(serviceType);
            }
        }
    }


    public static void Register<TService>(TService service, bool safe = true, 
        Action<TService> initializer = null) where TService : IRegistrable, new()
    {
        Debug.Log($"Registering {typeof(TService).Name}");
        var serviceType = typeof(TService);
        if (IsRegistered<TService>() && safe)
            throw new ServiceLocatorException($"{serviceType.Name} has been already registered.");
        initializer?.Invoke(service);

        Services[typeof(TService)] = service;
    }

    public static void RegisterSO<TService>(string path, 
        Action<TService> initializer = null) where TService : ScriptableObject, IRegistrable
    {
        var service = Resources.Load(path, typeof(TService)) as TService;
        if (IsRegistered<TService>())
            throw new ServiceLocatorException($"{typeof(TService).Name} has been already registered.");
        initializer?.Invoke(service);

        Services[typeof(TService)] = service;
        if (service is IInitializable initializable)
            initializable.Initialize();
    }

    public static TService Get<TService>(bool forced = false) where TService : IRegistrable, new()
    {
        var serviceType = typeof(TService);
        if (IsRegistered<TService>())
        {
            return (TService) Services[serviceType];
        } 
        if (!forced) 
            throw new ServiceLocatorException($"{serviceType.Name} hasn't been registered.");

        var service = serviceType.IsMonoBehaviour() ? 
            (TService) FindOrCreateMonoService(serviceType) : new TService();
        
        Register(service);
        return service;
    }

    public static bool IsRegistered(Type t)
    {
        return Services.ContainsKey(t);
    }

    public static bool IsRegistered<TService>()
    {
        return IsRegistered(typeof(TService));
    }

    static void RegisterNewInstance(Type serviceType)
    {
        var service = Activator.CreateInstance(serviceType);
        Services[serviceType] = service;
        if (service is IInitializable initializable)
            initializable.Initialize();
    }

    static object FindOrCreateMonoService(Type serviceType)
    {
        var inGameService = FindObjectOfType(serviceType);
        if (inGameService == null)
        {
            var newObject = new GameObject();
            newObject.AddComponent(serviceType);
            newObject.name = serviceType.Name;
            inGameService = newObject.GetComponent(serviceType);
        }
        Services[serviceType] = inGameService;
        if (inGameService is IInitializable initializable)
            initializable.Initialize();
        return inGameService;
    }
}

public class ServiceLocatorException : Exception
{
    public ServiceLocatorException(string message) : base(message) {}
}
