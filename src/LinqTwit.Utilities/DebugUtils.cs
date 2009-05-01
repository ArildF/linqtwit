using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;

namespace LinqTwit.Utilities
{

    public static class DebugUtils
    {
        /// <summary>
        /// Hook up all events on the object <paramref name="o"/>, displaying a string to the debugging 
        /// output whenever an event is raised.
        /// </summary>
        /// <param name="o">The object for which to monitor events.</param>
        [Conditional("DEBUG")]
        public static void DebugEvents(object o)
        {
            DebugEvents(o, null, null);
        }

        /// <summary>
        /// Hook up all events on the object <paramref name="o"/>, displaying a string formatted
        /// by <paramref name="formatString"/> to the debugging output whenever an event is raised.
        /// output whenever an event is raised.
        /// </summary>
        /// <param name="o">The object for which to monitor events.</param>
        /// <param name="formatString">A format string describing what is displayed every time an event is raised.
        /// <list type="bullet">
        /// <listheader>
        /// <term>Keywords</term>
        /// <description>Special keywords available.</description>
        /// </listheader>
        /// <item> <term>$EVENT</term> <description>Replaced with the name of the event raised (you almost always want this one).</description></item>
        /// <item><term>$TYPE</term><description> Replaced with the fully qualified type name of the object raising the event. </description></item>
        /// <item><term>$TIMESTAMP</term> <description>Replaced with the current time. </description></item>
        /// <item><term>$CALLER</term> <description>Replaced with the fully qualified method name 
        /// (and possibly line number if symbols are available) of the method that raised the event. </description></item>
        /// <item><term>$STACKTRACE</term><description>Replaced with a full stack trace of the code that eventually raised the event in question.</description></item>
        /// <item><term>An expression enclosed by curly braces.</term><description>An expression evaluating fields and properties starting on either 'e' 
        /// (the eventargs argument) or 'sender'(the sender argument).</description></item>
        /// </list>
        /// </param>
        [Conditional("DEBUG")]
        public static void DebugEvents(object o, string formatString)
        {
            DebugEvents(o, formatString, null);
        }

        /// <summary>
        /// Hook up all events matching the regular expression <paramref name="eventsRegex"/>on the object <paramref name="o"/>, displaying a string formatted
        /// by <paramref name="formatString"/> to the debugging output whenever an event is raised.
        /// output whenever an event is raised.
        /// </summary>
        /// <param name="o">The object for which to monitor events.</param>
        /// <param name="formatString">A format string describing what is displayed every time an event is raised.
        /// <list type="bullet">
        /// <listheader>
        /// <term>Keywords</term>
        /// <description>Special keywords available.</description>
        /// </listheader>
        /// <item> <term>$EVENT</term> <description>Replaced with the name of the event raised (you almost always want this one).</description></item>
        /// <item><term>$TYPE</term><description> Replaced with the fully qualified type name of the object raising the event. </description></item>
        /// <item><term>$TIMESTAMP</term> <description>Replaced with the current time. </description></item>
        /// <item><term>$CALLER</term> <description>Replaced with the fully qualified method name 
        /// (and possibly line number if symbols are available) of the method that raised the event. </description></item>
        /// <item><term>$STACKTRACE</term><description>Replaced with a full stack trace of the code that eventually raised the event in question.</description></item>
        /// <item><term>An expression enclosed by curly braces.</term><description>An expression evaluating fields and properties starting on either 'e' 
        /// (the eventargs argument) or 'sender'(the sender argument).</description></item>
        /// </list>
        /// </param>
        /// <param name="eventsRegex">A regular expression to match the events to be monitored.</param>
        [Conditional("DEBUG")]
        public static void DebugEvents(object o, string formatString, string eventsRegex)
        {
            // Find all the events on the object.
            foreach (EventInfo info in o.GetType().GetEvents(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public))
            {
                if (eventsRegex == null || Regex.IsMatch(info.Name, eventsRegex))
                {
                    EventReceiver receiver = new EventReceiver(o, info, formatString);
                }
            }
        }

        private class EventReceiver
        {
            public EventReceiver(object obj, EventInfo eventInfo, string formatString)
            {
                this.obj = obj;
                this.eventInfo = eventInfo;
                this.formatString = formatString;

                this.EmitAndHookUpEventHandler();
            }

            private void EmitAndHookUpEventHandler()
            {
                MethodInfo mi = this.eventInfo.EventHandlerType.GetMethod("Invoke");

                // Get the parameter list
                Type[] baseMethodParmTypes = Array.ConvertAll<ParameterInfo, Type>(mi.GetParameters(), delegate(ParameterInfo par)
                {
                    return par.ParameterType;
                });

                // We can't deal with anything else right now.
                if (baseMethodParmTypes.Length != 2)
                {
                    throw new ArgumentException("Only events with the standard signature can be used (object sender, object args)");
                }

                // since it's an instance method, the first argument needs to be of type EventReceiver
                Type[] parmTypes = new Type[baseMethodParmTypes.Length + 1];
                parmTypes[0] = typeof(EventReceiver);
                Array.Copy(baseMethodParmTypes, 0, parmTypes, 1, baseMethodParmTypes.Length);

                // Create the DM so we can emit code to it.
                DynamicMethod m = new DynamicMethod(this.eventInfo.Name + "Handler", typeof(void), parmTypes, typeof(EventReceiver));
                ILGenerator generator = m.GetILGenerator();

                // emit call to the WriteEventInfo method
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldarg_2);
                MethodInfo eventReceiverMethod = typeof(EventReceiver).GetMethod("WriteEventInfo", BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new Type[] { typeof(object), typeof(object) }, new ParameterModifier[] { });
                generator.Emit(OpCodes.Call, eventReceiverMethod);

                // don't forget the ret
                generator.Emit(OpCodes.Ret);

                Delegate del = m.CreateDelegate(this.eventInfo.EventHandlerType, this);

                this.eventInfo.AddEventHandler(this.obj, del);
            }

            private void WriteEventInfo(object sender, object e)
            {
                string eventInfoString;

                // don't waste time with the regexes if the user doesn't have such needs.
                if (this.formatString == null)
                {
                    eventInfoString = String.Format("Event raised: {0}.{1}", this.obj.GetType().FullName, this.eventInfo.Name);
                }
                else
                {
                    eventInfoString = this.FormatEventInfoString(sender, e);
                }

                Debug.WriteLine(eventInfoString);
            }

            private string FormatEventInfoString(object sender, object e)
            {
                // match $EVENT
                string eventInfoString = EventRegex.Replace(this.formatString, this.eventInfo.Name);

                // match $TYPE
                eventInfoString = TypeRegex.Replace(eventInfoString, this.obj.GetType().FullName);

                // match $TIMESTAMP
                eventInfoString = TimeStampRegex.Replace(eventInfoString, DateTime.Now.ToString("HH:mm:ss.ffffff"));

                // match $CALLER
                if (CallerRegex.IsMatch(eventInfoString))
                {
                    StackTrace stackTrace = new StackTrace(0, true);
                    int frameIndex = FindCallerFrameIndex(stackTrace);
                    StackFrame frame = stackTrace.GetFrame(frameIndex);
                    MethodBase method = frame.GetMethod();
                    string caller = String.Format("{0}.{1}", method.DeclaringType.FullName, method.Name);
                    int lineNumber = frame.GetFileLineNumber();
                    if (lineNumber > 0)
                    {
                        caller += ", line " + lineNumber;
                    }
                    eventInfoString = CallerRegex.Replace(eventInfoString, caller);
                }

                // match $STACKTRACE
                if (StackTraceRegex.IsMatch(eventInfoString))
                {
                    StackTrace stackTrace = new StackTrace(0, true);
                    int frameIndex = FindCallerFrameIndex(stackTrace);
                    stackTrace = new StackTrace(frameIndex, true);

                    string traceString = stackTrace.ToString();
                    eventInfoString = StackTraceRegex.Replace(eventInfoString, traceString);
                }

                // match expression
                eventInfoString = ExpressionRegex.Replace(eventInfoString, delegate(Match match)
                {
                    return this.ParseExpression(match, sender, e);
                });

                return eventInfoString;
            }

            /// <summary>
            /// Find the first frame on the stack belonging to the object we're getting the events from.
            /// </summary>
            /// <param name="stackTrace"></param>
            /// <returns></returns>
            private int FindCallerFrameIndex(StackTrace stackTrace)
            {
                for (int i = 0; i < stackTrace.FrameCount; i++)
                {
                    StackFrame frame = stackTrace.GetFrame(i);
                    MethodBase method = frame.GetMethod();
                    if (method.DeclaringType != null && method.DeclaringType.IsAssignableFrom(this.obj.GetType()))
                    {
                        return i;
                    }
                }

                throw new Exception("Cannot find caller stack frame");
            }

            /// <summary>
            /// Parses an expression in curlies.
            /// </summary>
            /// <param name="match"></param>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            /// <returns></returns>
            private string ParseExpression(Match match, object sender, object e)
            {
                string expression = match.Groups["Expression"].Value;

                string[] components = expression.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                components = Array.ConvertAll<string, string>(components, delegate(string input)
                {
                    return input.Trim();
                });

                if (components.Length == 0)
                {
                    return "";
                }

                // We put this in an object as well so we can evaluate the first "e" or "sender" as a field expression.
                object result = new Args(sender, e);

                foreach (string component in components)
                {
                    // is this a property?
                    PropertyInfo propertyInfo = result.GetType().GetProperty(component, ComponentFlags);
                    if (propertyInfo != null)
                    {
                        result = propertyInfo.GetValue(result, null);
                    }
                    else
                    {
                        // nope, a field?
                        FieldInfo fieldInfo = result.GetType().GetField(component, ComponentFlags);
                        if (fieldInfo != null)
                        {
                            result = fieldInfo.GetValue(result);
                        }
                        else
                        {
                            // Oh well :-(
                            return String.Format("{{Cannot find field or property {0} on object of type {1}}}", component, result.GetType());
                        }
                    }
                }

                return result.ToString();
            }

            /// <summary>
            /// Used to simplify the expression evaluation so we don't have to special case the first expression component.
            /// </summary>
            private class Args
            {
                public readonly object Sender;
                public readonly object E;

                public Args(object sender, object e)
                {
                    this.Sender = sender;
                    this.E = e;
                }
            }

            private const BindingFlags ComponentFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;

            private static readonly Regex EventRegex = new Regex(@"\$EVENT", RegexOptions.IgnoreCase);
            private static readonly Regex TypeRegex = new Regex(@"\$TYPE", RegexOptions.IgnoreCase);
            private static readonly Regex TimeStampRegex = new Regex(@"\$TIMESTAMP", RegexOptions.IgnoreCase);
            private static readonly Regex CallerRegex = new Regex(@"\$CALLER", RegexOptions.IgnoreCase);
            private static readonly Regex StackTraceRegex = new Regex(@"\$STACKTRACE", RegexOptions.IgnoreCase);
            private static readonly Regex ExpressionRegex = new Regex(@"\{(?<Expression>[^\}]*?)\}", RegexOptions.IgnoreCase);

            private readonly object obj;
            private readonly string formatString;
            private readonly EventInfo eventInfo;
        }
    }
}
