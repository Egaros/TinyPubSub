﻿/*
 * The MIT License (MIT)
 * Copyright (c) 2016 Johan Karlsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights 
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
 * copies of the Software, and to permit persons to whom the Software is furnished 
 * to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

namespace TinyPubSubLib
{
    public static class TinyPubSub
    {
        private static Dictionary<string, List<ISubscription>> _channels = new Dictionary<string, List<ISubscription>>();

        private static List<ISubscription> GetOrCreateChannel(string channel)
        {
            List<ISubscription> subscriptions;
            if (!_channels.ContainsKey(channel))
            {
                subscriptions = new List<ISubscription>();
                _channels.Add(channel, subscriptions);
            }
            else
            {
                subscriptions = _channels[channel];
            }
            return subscriptions;
        }

        private static ISubscription CreateSubscription(object owner, string channel, Action action, bool disableAfterFirstUse = false)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<string>(owner, action);
            subscription.RemoveAfterUse = disableAfterFirstUse;
            current.Add(subscription);
            return subscription;
        }

        private static ISubscription CreateSubscription<T>(object owner, string channel, Action<T> action, bool disableAfterFirstUse = false)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<T>(owner, action);
            subscription.RemoveAfterUse = disableAfterFirstUse;
            current.Add(subscription);
            return subscription;
        }

        private static ISubscription CreateSubscription<T>(object owner, string channel, Action<T, TinyEventArgs> action, bool disableAfterFirstUse = false)
        {
            var current = GetOrCreateChannel(channel);
            var subscription = new Subscription<T>(owner, action);
            subscription.RemoveAfterUse = disableAfterFirstUse;
            current.Add(subscription);
            return subscription;
        }

        /// <summary>
        /// Subscribe to a channel
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        public static string Subscribe(string channel, Action action)
        {
            var subscription = CreateSubscription(null, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        public static string Subscribe(string channel, Action<string> action)
        {
            var subscription = CreateSubscription<string>(null, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel
        /// </summary>
        /// <param name="owner">The owner of the subscription</param> 
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        /// <remarks>The owner can be used to make a mass-unsubscription by 
        /// calling Unsubcribe and pass the same object.</remarks>
        public static string Subscribe(object owner, string channel, Action action)
        {
            var subscription = CreateSubscription(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument
        /// </summary>
        /// <param name="owner">The owner of the subscription</param> 
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <returns>A tag that can be used to unsubscribe</returns>
        /// <remarks>The owner can be used to make a mass-unsubscription by 
        /// calling Unsubcribe and pass the same object.</remarks>
        public static string Subscribe(object owner, string channel, Action<string> action)
        {
            var subscription = CreateSubscription<string>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(string channel, Action<T> action)
        {
            var subscription = CreateSubscription<T>(null, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// /// <param name="owner">The owner of the subscription</param> 
        /// <param name="channel">The channel name</param>
        /// <param name="action">The action to run</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(object owner, string channel, Action<T> action)
        {
            var subscription = CreateSubscription<T>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner and TinyEventArgs to be able to cancle execution and specify if the event has been handled
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="owner">Owner.</param>
        /// <param name="channel">Channel.</param>
        /// <param name="action">Action with T and TinyEventArgs for execution handling and publishreturn.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(object owner, string channel, Action<T, TinyEventArgs> action)
        {
            var subscription = CreateSubscription<T>(owner, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Subscribe to a channel that sends an argument with specified owner and TinyEventArgs to be able to cancle execution and specify if the event has been handled
        /// </summary>
        /// <returns>The subscribe.</returns>
        /// <param name="channel">Channel.</param>
        /// <param name="action">Action with T and TinyEventArgs for execution handling and publishreturn.</param>
        /// <typeparam name="T">The type to subscribe to.</typeparam>
        public static string Subscribe<T>(string channel, Action<T, TinyEventArgs> action)
        {
            var subscription = CreateSubscription<T>(null, channel, action);
            return subscription.Tag;
        }

        /// <summary>
        /// Unsubscribes to a channel based on a tag
        /// </summary>
        /// <param name="tag"></param>
        public static void Unsubscribe(string tag)
        {
            foreach (var channel in _channels)
            {
                foreach (var subscription in channel.Value.ToList())
                {
                    if (subscription.Tag == tag)
                    {
                        channel.Value.Remove(subscription);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribes to a channel based on an object owner
        /// </summary>
        /// <param name="owner"></param>
		public static void Unsubscribe(object owner)
        {
            if (owner == null)
            {
                return;
            }

            foreach (var channel in _channels)
            {
                foreach (var subscription in channel.Value.ToList())
                {
                    if (subscription.Owner == owner)
                    {
                        channel.Value.Remove(subscription);
                    }
                }
            }
        }

        /// <summary>
        /// Publish an event the specified channel with instance argument.
        /// </summary>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance to pass to the receiver.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Publish<T>(string channel, T instance, Action<Exception, ISubscription> OnError = null)
        {
            PublishControlled<T>(channel, instance, OnError);
        }

        /// <summary>
        /// Publish an event the specified channel with instance argument and returns if the event is handled.
        /// </summary>
        /// <returns>The controlled.</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance to pass to the receiver.</param>
        public static TinyEventArgs PublishControlled(string channel, string instance, Action<Exception, ISubscription> OnError = null)
        {
            return PublishControlled<string>(channel, instance, OnError);
        }

        /// <summary>
        /// Publish an event the specified channel with instance argument and returns if the event is handled.
        /// </summary>
        /// <returns>The controlled.</returns>
        /// <param name="channel">The channel name</param>
        /// <param name="instance">Instance to pass to the receiver.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static TinyEventArgs PublishControlled<T>(string channel, T instance, Action<Exception, ISubscription> OnError = null)
        {
            var returnEventArgs = new TinyEventArgs();

            if (string.IsNullOrWhiteSpace(channel))
            {
                throw new ArgumentException("You have to specify a channel to publish to");
            }

            if (_channels.ContainsKey(channel))
            {
                var current = _channels[channel];

                foreach (var subscription in current.OfType<Subscription<T>>().ToList())
                {
                    try
                    {
                        if (subscription.ActionWithArgumentAndArgs != null)
                        {
                            subscription.ActionWithArgumentAndArgs.Invoke(instance, returnEventArgs);
                        }
                        else
                        {
                            var hasBeenHandled = false;

                            if (subscription.Action != null)
                            {
                                subscription.Action.Invoke();
                                hasBeenHandled = true;
                            }
                            else if (subscription.ActionWithArgument != null)
                            {
                                subscription.ActionWithArgument.Invoke(instance);
                                hasBeenHandled = true;
                            }

                            returnEventArgs.Handled = hasBeenHandled;
                        }
                    }
                    catch (Exception ex)
                    {
                        OnError?.Invoke(ex, subscription);
					    SendException(ex, subscription.Tag);
                    }

                    if (subscription.RemoveAfterUse)
                    {
						Unsubscribe(subscription.Tag);
                    }

                    if (returnEventArgs.HaltExecution)
                    {
						return returnEventArgs;
                    }
                }

                // Concept code - fall back to calling each with object
                // this is the way we need to do it for allowing attribute
                // subscription.
                if (typeof(T) != typeof(object))
                {
					PublishControlled<object>(channel, instance, OnError);
                }
            }
            return returnEventArgs;
        }

        private static void SendException(Exception ex, string tag)
        {
            var message = new TinyException()
            {
                Message = "Error sending event to receiver: " + ex.Message,
                InnerException = ex,
                SubscriptionTag = tag
            };
            Publish<TinyException>(TinyException.DefaultChannel, message);
        }

        /// <summary>
        /// Publish an event the specified channel.
        /// </summary>
        /// <param name="channel">The channel name</param>
        public static void Publish(string channel, string argument = default(string), Action<Exception, ISubscription> OnError = null)
        {
            Publish<string>(channel, argument, OnError);
        }

        /// <summary>
        /// Publish using Task.Run
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="argument">An optional parameter</param>
        /// <remarks>This method is not blocking, it simply uses a Task.Run(() => Publish(...)) internally
        /// to hand of the call to be handled by someone else.</remarks>
        public static void PublishAsTask(string channel, string argument = default(string), Action<Exception, ISubscription> OnError = null)
        {
            // Add to delayed handle queue
            Task.Run(() => Publish(channel, argument, OnError)).ConfigureAwait(false);
        }

        /// <summary>
        /// Publish async
        /// </summary>
        /// <param name="channel">The channel to publish to</param>
        /// <param name="argument">An optional parameter</param>
        /// <returns>A task</returns>
        public static async Task PublishAsync(string channel, string argument = default(string), Action<Exception, ISubscription> OnError = null)
        {
            await Task.Run(() => Publish(channel, argument, OnError));
        }

        /// <summary>
        /// Clears all channels
        /// </summary>
        public static void Clear()
        {
            _channels.Clear();
        }

        /// <summary>
        /// Scans an object after attributes to hook up to TinyPubSub
        /// </summary>
        /// <param name="obj">The object to scan</param>
        public static void Register(object obj)
        {
            var typeInfo = IntrospectionExtensions.GetTypeInfo(obj.GetType());

            foreach (var method in typeInfo.DeclaredMethods)
            {
                var attributes = method.GetCustomAttributes(typeof(TinySubscribeAttribute)).OfType<TinySubscribeAttribute>();

                foreach (var attribute in attributes)
                {
                    var channel = attribute.Channel;

                    if (method.GetParameters().Any())
                    {
                        // Concept code for subscriptions
                        var firstParam = method.GetParameters().First();
                        var paramType = firstParam.ParameterType;
                        TinyPubSub.Subscribe<object>(obj, channel, (data) => method.Invoke(obj, new object[] { data }));
                    }
                    else
                    {
                        // Register without parameters since the target method has none
						TinyPubSub.Subscribe(obj, channel, () => method.Invoke(obj, null));
                    }
                }
            }
        }

        public static void Deregister(object obj)
        {
            TinyPubSub.Unsubscribe(obj);
        }
    }
}
