# TinyPubSub
Worlds smallest pub/sub thingy created mostly for Xamarin Forms but should also work else where...

## WHY SHOULD I USE IT?

This lib should be used when you want to easily register to events within a small app. It's not meant for data transfer (at least not at this point), it's not thread safe and it's not finished. :)

### EXAMPLE
I have a view that shows ducks. This is my main view. When I edit ducks on another view and the main page is covered I still want the main view to get new ducks before I return to it. I don't want the MainPage to start loading when it gets displayed. Then the main view can listen for the "ducks-added" event and run an action when that happens. Later on when I create a new function in the system, I can trust that if I publish an event on the "ducks-added" channel, all my other views subscribing to that event will get notified.

And by following some patterns regarding the NavigationPage(...) we can also make sure that the subscriptions are removed when the view go out of scope.

It's designed with MVVM in mind. Subscription to new events should be done in the ViewModel and the Unsubscription should be made automatically when pages are popped (see usage).

It's not meant to solve world problems so if you want a robust and mature pub/sub framework then there are plenty others out there to use. This is bare metal.

## STATE

Alpha

## NUGET

https://www.nuget.org/packages/tinypubsub

Added package for profile 259

## USAGE

To subscribe, simply register what "channel" (we call them channels) you would like to subscribe to.

```c#
TinyPubSub.Subscribe("new-duck-added", () => { RebindDuckGui(); });
```

And in another part of you application, publish events to execute the actions that are registered for that channel.

```c#
TinyPubSub.Publish("new-duck-added");
```

### WHAT ABOUT MEMORY ISSUES?

Oh, yes... Working on that. Currently following up two possible ways of making it simple.

#### Plan A - tags

When subscribing you get a tag.

```c#
var tag = TinyPubSub.Subscribe("new-duck-added", () => { RebindDuckGui(); });
```

And when you are done you unsubscribe with that tag.

```c#
TinyPubSub.Unsubscribe(tag);
```

#### Plan B - object refs

This is a more suitable option for Xamarin MVVM (which is really the reason for this projects existance). I don't like having to keep track of tags. So instead we pass a reference to an object that counts as the owner of the subscription. Usually this and most usually a ViewModel. This way we can subscribe to several channels.

```c#
TinyPubSub.Subscribe(this, "new-duck-added", () => { RebindDuckGui(); });
TinyPubSub.Subscribe(this, "old-duck-removed", () => { RebindDuckGui(); });
```

And when the view is done (if we're talking MVVM) then the unsubscription could look like this.

```c#
TinyPubSub.Unsubscribe(this);
```

Or specifically in Xamarin Forms

```c#
TinyPubSub.Unsubscribe(this.BindingContext); // if this is a View and the Binding context the view model
```

The tricky part is still knowing when the view is done. One way is to hook up to the navigation page Popped event.

```c#
// The root page of your application
var navPage = new NavigationPage(new MainView());
navPage.Popped += (object sender, NavigationEventArgs e) => TinyPubSub.Unsubscribe(e.Page.BindingContext);
MainPage = navPage;
```

This works as long as PopToRoot isn't called and you are more than one level deep in the navigation stack. There is also a NavigationPage.PoppedToRoot event, but looking at the Xamarin Forms code it simply clears the children without calling popped for each page. I've started a thread about this at the xamarin forums.
