using System;
using System.Linq;
using System.Threading.Tasks;

namespace SummonMyStrength.Api
{
    public static class EventExtensions
    {
        public static async Task InvokeAsync(this Func<Task> @event)
        {
            if (@event != null)
            {
                await Task.WhenAll(@event.GetInvocationList().Select(x => ((Func<Task>)x).Invoke()));
            }
        }

        public static async Task InvokeAsync<TParam1>(this Func<TParam1, Task> @event, TParam1 parameter1)
        {
            if (@event != null)
            {
                try
                {
                    await Task.WhenAll(@event.GetInvocationList().Select(x => ((Func<TParam1, Task>)x).Invoke(parameter1)));
                }
                catch
                {
                }
            }
        }

        public static async Task InvokeAsync<TParam1, TParam2>(this Func<TParam1, TParam2, Task> @event, TParam1 parameter1, TParam2 parameter2)
        {
            if (@event != null)
            {
                await Task.WhenAll(@event.GetInvocationList().Select(x => ((Func<TParam1, TParam2, Task>)x).Invoke(parameter1, parameter2)));
            }
        }
    }
}
