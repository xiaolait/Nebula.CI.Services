using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Volo.Abp.Collections;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;

namespace Nebula.Abp.EventBus.InMemDistributed
{
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IDistributedEventBus), typeof(InMemDistributedEventBus))]
    public class InMemDistributedEventBus : IDistributedEventBus, ISingletonDependency
    {
        private readonly ILocalEventBus _localEventBus;
        private readonly Dictionary<string, List<Type>> _eventTypeCollection;
        protected IServiceScopeFactory _serviceScopeFactory { get; }
        protected AbpDistributedEventBusOptions _abpDistributedEventBusOptions { get; }

        public InMemDistributedEventBus(
            ILocalEventBus localEventBus,
            IServiceScopeFactory serviceScopeFactory,
            IOptions<AbpDistributedEventBusOptions> distributedEventBusOptions)
        {
            _localEventBus = localEventBus;
            _serviceScopeFactory = serviceScopeFactory;
            _abpDistributedEventBusOptions = distributedEventBusOptions.Value;
            _eventTypeCollection = GetEventTypeCollection(distributedEventBusOptions.Value.Handlers);
            Subscribe(distributedEventBusOptions.Value.Handlers);
        }

        public virtual void Subscribe(ITypeList<IEventHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                var interfaces = handler.GetInterfaces();
                foreach (var @interface in interfaces)
                {
                    if (!typeof(IEventHandler).GetTypeInfo().IsAssignableFrom(@interface))
                    {
                        continue;
                    }

                    var genericArgs = @interface.GetGenericArguments();
                    if (genericArgs.Length == 1)
                    {
                        Subscribe(genericArgs[0], new IocEventHandlerFactory(_serviceScopeFactory, handler));
                    }
                }
            }
        }

        public virtual IDisposable Subscribe<TEvent>(IDistributedEventHandler<TEvent> handler) where TEvent : class
        {
            return Subscribe(typeof(TEvent), handler);
        }

        public IDisposable Subscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            return _localEventBus.Subscribe(action);
        }

        public IDisposable Subscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            return _localEventBus.Subscribe(handler);
        }

        public IDisposable Subscribe<TEvent, THandler>() where TEvent : class where THandler : IEventHandler, new()
        {
            return _localEventBus.Subscribe<TEvent, THandler>();
        }

        public IDisposable Subscribe(Type eventType, IEventHandler handler)
        {
            return _localEventBus.Subscribe(eventType, handler);
        }

        public IDisposable Subscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            return _localEventBus.Subscribe<TEvent>(factory);
        }

        public IDisposable Subscribe(Type eventType, IEventHandlerFactory factory)
        {
            return _localEventBus.Subscribe(eventType, factory);
        }

        public void Unsubscribe<TEvent>(Func<TEvent, Task> action) where TEvent : class
        {
            _localEventBus.Unsubscribe(action);
        }

        public void Unsubscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : class
        {
            _localEventBus.Unsubscribe(handler);
        }

        public void Unsubscribe(Type eventType, IEventHandler handler)
        {
            _localEventBus.Unsubscribe(eventType, handler);
        }

        public void Unsubscribe<TEvent>(IEventHandlerFactory factory) where TEvent : class
        {
            _localEventBus.Unsubscribe<TEvent>(factory);
        }

        public void Unsubscribe(Type eventType, IEventHandlerFactory factory)
        {
            _localEventBus.Unsubscribe(eventType, factory);
        }

        public void UnsubscribeAll<TEvent>() where TEvent : class
        {
            _localEventBus.UnsubscribeAll<TEvent>();
        }

        public void UnsubscribeAll(Type eventType)
        {
            _localEventBus.UnsubscribeAll(eventType);
        }

        public async Task PublishAsync<TEvent>(TEvent eventData)
            where TEvent : class
        {
            await PublishAsync(eventData.GetType(), eventData);
        }

        public async Task PublishAsync(Type eventType, object eventData)
        {
            var eventName = eventType.GetCustomAttribute<EventNameAttribute>().Name;
            if (!_eventTypeCollection.ContainsKey(eventName)) return;
            var eventTypeList = _eventTypeCollection[eventName];
            var eventJson = JsonConvert.SerializeObject(eventData);
            foreach(var eType in eventTypeList)
            {
                var newEventData = JsonConvert.DeserializeObject(eventJson, eType);
                await _localEventBus.PublishAsync(eType, newEventData);
            }
        }

        private Dictionary<string, List<Type>> GetEventTypeCollection(ITypeList<IEventHandler> handlers)
        {
            var eventTypeCollection = new Dictionary<string, List<Type>>();

            foreach (var handler in handlers)
            {
                var handleEvents = handler.GetMethods();
                foreach (var hanldeEvent in handleEvents)
                {
                    if (hanldeEvent.Name != "HandleEventAsync") continue;
                    var parameters = hanldeEvent.GetParameters();
                    var parameterType = parameters?[0].ParameterType;
                    if (parameterType == null) continue;
                    var eventNameAttribute = parameterType.GetCustomAttribute(typeof(EventNameAttribute), false) as EventNameAttribute;
                    if (eventNameAttribute == null) continue;
                    if (!eventTypeCollection.ContainsKey(eventNameAttribute.Name))
                        eventTypeCollection[eventNameAttribute.Name] = new List<Type>();
                    eventTypeCollection[eventNameAttribute.Name].AddIfNotContains(parameterType);
                }
            }

            return eventTypeCollection;
        }
    }
}