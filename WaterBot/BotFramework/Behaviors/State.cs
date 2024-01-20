using System;
using System.Collections.Generic;
using StardewModdingAPI.Events;

namespace BotFramework.Behaviors
{
    /// <inheritdoc cref="IState"/>
    public abstract class State : IState
    {
        /// <summary>
        /// Prevents external states from activating.
        /// </summary>
        protected bool _lock = false;

        /// <summary>
        /// Whether this state is active.
        /// </summary>
        protected bool _active = false;

        /// <summary>
        /// Instantiates a new state.
        /// </summary>
        /// <param name="active">Whether this state starts off active.</param>
        public State(bool active = false)
        {
            this._active = active;
        }

        /// <inheritdoc cref="IState.Activate"/>
        public virtual void Activate()
        {
        }

        /// <inheritdoc cref="IState.Deactivate"/>
        public virtual void Deactivate()
        {
        }

        /// <inheritdoc cref="IState.GetKey"/>
        public abstract string GetKey();

        /// <inheritdoc cref="IState.GetName"/>
        public abstract string GetName();

        /// <inheritdoc cref="IState.GetBehaviors"/>
        public virtual IList<IBehavior> GetBehaviors()
        {
            return new List<IBehavior>();
        }

        /// <inheritdoc cref="IState.GetBehaviors"/>
        public virtual IList<IBehavior> GetBehaviors(params IBehavior[] behaviors)
        {
            IList<IBehavior> list = new List<IBehavior>();

            foreach (IBehavior behavior in behaviors)
            {
                list.Add(behavior);
            }

            return list;
        }

        /// <inheritdoc cref="IState.ShouldCheckOnGameTick"/>
        public virtual bool ShouldCheckOnGameTick(IState active)
        {
            return false;
        }

        /// <inheritdoc cref="IState.CheckGameTick"/>
        public virtual int CheckGameTick(
            IState active,
            object sender,
            UpdateTickedEventArgs args
        )
        {
            return 0;
        }

        /// <inheritdoc cref="IState.ShouldCheckOnDayStart"/>
        public virtual bool ShouldCheckOnDayStart(IState active)
        {
            return false;
        }

        /// <inheritdoc cref="IState.CheckDayStart"/>
        public virtual int CheckDayStart(
            IState active,
            object sender,
            DayStartedEventArgs args
        )
        {
            return 0;
        }

        /// <inheritdoc cref="IState.ShouldCheckOnButtonPress"/>
        public virtual bool ShouldCheckOnButtonPress(IState active)
        {
            return false;
        }

        /// <inheritdoc cref="IState.CheckButtonPressed"/>
        public virtual int CheckButtonPressed(
            IState active,
            object sender,
            ButtonPressedEventArgs args
        )
        {
            return 0;
        }

        /// <inheritdoc cref="IState.ShouldCheckOnWarp"/>
        public virtual bool ShouldCheckOnWarp(IState active)
        {
            return false;
        }

        /// <inheritdoc cref="IState.CheckWarped"/>
        public virtual int CheckWarped(
            IState active,
            object sender,
            WarpedEventArgs args
        )
        {
            return 0;
        }

        /// <inheritdoc cref="IState.ShouldCheckBeforeAction"/>
        public virtual bool ShouldCheckBeforeAction(IState active)
        {
            return false;
        }

        /// <inheritdoc cref="IState.CheckBeforeAction"/>
        public virtual int CheckBeforeAction(IState active)
        {
            return 0;
        }

        /// <inheritdoc cref="IState.ShouldCheckAfterAction"/>
        public virtual bool ShouldCheckAfterAction(IState active)
        {
            return false;
        }

        /// <inheritdoc cref="IState.CheckAfterAction"/>
        public virtual int CheckAfterAction(IState active)
        {
            return 0;
        }

        /// <inheritdoc cref="IState.ShowsActivationMessage"/>
        public virtual bool ShowsActivationMessage()
        {
            return false;
        }

        /// <inheritdoc cref="IState.GetActivationMessage"/>
        public virtual string GetActivationMessage()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IState.GetActivationIcon"/>
        public virtual string GetActivationIcon()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IState.ShowsDeactivationMessage"/>
        public virtual bool ShowsDeactivationMessage()
        {
            return false;
        }

        /// <inheritdoc cref="IState.GetDeactivationMessage"/>
        public virtual string GetDeactivationMessage()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IState.GetDeactivationIcon"/>
        public virtual string GetDeactivationIcon()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IState.IsLocked"/>
        public bool IsLocked()
        {
            return this._lock;
        }

        /// <inheritdoc cref="IState.Equals"/>
        public bool Equals(IState other)
        {
            return this.GetKey() == other.GetKey();
        }
    }
}
