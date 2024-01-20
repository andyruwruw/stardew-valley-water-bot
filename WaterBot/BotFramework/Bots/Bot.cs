using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI.Events;
using BotFramework.Behaviors;
using BotFramework.Characters;

namespace BotFramework.Bots
{
    /// <inheritdoc cref="IBot"/>
    public abstract class Bot : IBot
    {
        /// <summary>
        /// Reference to the character this bot controls.
        /// </summary>
        protected CharacterController _characterController;

        /// <summary>
        /// Whether the bot is currently running.
        /// </summary>
        protected bool _active;

        /// <summary>
        /// Current <see cref="IState"/> of the bot.
        /// </summary>
        protected IState _currentState;

        /// <summary>
        /// Dictionary of bot <see cref="IState">IStates</see>.
        /// </summary>
        protected IDictionary<string, IState> _states;

        /// <summary>
        /// Instantiates a new bot.
        /// </summary>
        public Bot(Character character)
        {
            this._characterController = new CharacterController(character);
            this._active = this.ShouldStartActive();
            this._states = this.IniticalStates();
            this._currentState = this.InitialDefaultState();
        }

        /// <summary>
        /// Handles the deattatchment of the bot from the event loop.
        /// </summary>
        ~Bot()
        {
            BotManager.Detatch(this);
        }

        /// <inheritdoc cref="IBot.GetKey"/>
        public abstract string GetKey();

        /// <inheritdoc cref="IBot.GetName"/>
        public abstract string GetName();

        /// <inheritdoc cref="IBot.GetDescription"/>
        public virtual string GetDescription()
        {
            return "No description provided.";
        }

        /// <inheritdoc cref="IBot.Start"/>
        public virtual void Start()
        {
            this._active = true;
        }

        /// <inheritdoc cref="IBot.Stop"/>
        public virtual void Stop()
        {
            this._active = true;
        }

        /// <inheritdoc cref="IBot.GameTick"/>
        public virtual void GameTick(
            object sender,
            UpdateTickedEventArgs args
        )
        {
            if (this._currentState.IsLocked())
            {
                if (this._currentState.ShouldCheckOnGameTick(this._currentState))
                {
                    int status = this._currentState.CheckGameTick(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status != 0)
                        this._currentState = this.InitialDefaultState();
                }
                else
                {
                    return;
                }
            }

            if (this._currentState.IsLocked())
                return;

            foreach (KeyValuePair<string, IState> state in this._states)
            {
                if (state.Value.ShouldCheckOnGameTick(this._currentState))
                {
                    int status = state.Value.CheckGameTick(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status == 0 && state.Value == this._currentState)
                        this._currentState = this.InitialDefaultState();
                    else if (status == 1 && state.Value != this._currentState)
                        this._currentState = state.Value;
                }
            }
        }

        /// <inheritdoc cref="IBot.DayStarted"/>
        public virtual void DayStarted(
            object sender,
            DayStartedEventArgs args
        )
        {
            if (this._currentState.IsLocked())
            {
                if (this._currentState.ShouldCheckOnDayStart(this._currentState))
                {
                    int status = this._currentState.CheckDayStart(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status != 0)
                        this._currentState = this.InitialDefaultState();
                }
                else
                {
                    return;
                }
            }

            if (this._currentState.IsLocked())
                return;

            foreach (KeyValuePair<string, IState> state in this._states)
            {
                if (state.Value.ShouldCheckOnDayStart(this._currentState))
                {
                    int status = state.Value.CheckDayStart(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status == 0 && state.Value == this._currentState)
                        this._currentState = this.InitialDefaultState();
                    else if (status == 1 && state.Value != this._currentState)
                        this._currentState = state.Value;
                }
            }
        }

        /// <inheritdoc cref="IBot.ButtonPressed"/>
        public virtual void ButtonPressed(
            object sender,
            ButtonPressedEventArgs args
        )
        {
            if (this._currentState.IsLocked())
            {
                if (this._currentState.ShouldCheckOnButtonPress(this._currentState))
                {
                    int status = this._currentState.CheckButtonPressed(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status != 0)
                        this._currentState = this.InitialDefaultState();
                }
                else
                {
                    return;
                }
            }

            if (this._currentState.IsLocked())
                return;

            foreach (KeyValuePair<string, IState> state in this._states)
            {
                if (state.Value.ShouldCheckOnButtonPress(this._currentState))
                {
                    int status = state.Value.CheckButtonPressed(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status == 0 && state.Value == this._currentState)
                        this._currentState = this.InitialDefaultState();
                    else if (status == 1 && state.Value != this._currentState)
                        this._currentState = state.Value;
                }
            }
        }

        /// <inheritdoc cref="IBot.Warped"/>
        public virtual void Warped(
            object sender,
            WarpedEventArgs args
        )
        {
            if (this._currentState.IsLocked())
            {
                if (this._currentState.ShouldCheckOnWarp(this._currentState))
                {
                    int status = this._currentState.CheckWarped(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status != 0)
                        this._currentState = this.InitialDefaultState();
                }
                else
                {
                    return;
                }
            }

            if (this._currentState.IsLocked())
                return;

            foreach (KeyValuePair<string, IState> state in this._states)
            {
                if (state.Value.ShouldCheckOnWarp(this._currentState))
                {
                    int status = state.Value.CheckWarped(
                        this._currentState,
                        sender,
                        args
                    );

                    if (status == 0 && state.Value == this._currentState)
                        this._currentState = this.InitialDefaultState();
                    else if (status == 1 && state.Value != this._currentState)
                        this._currentState = state.Value;
                }
            }
        }

        /// <inheritdoc cref="IBot.BeforeAction"/>
        public virtual void BeforeAction()
        {
            if (this._currentState.IsLocked())
            {
                if (this._currentState.ShouldCheckBeforeAction(this._currentState))
                {
                    int status = this._currentState.CheckBeforeAction(this._currentState);

                    if (status != 0)
                        this._currentState = this.InitialDefaultState();
                }
                else
                {
                    return;
                }
            }

            if (this._currentState.IsLocked())
                return;

            foreach (KeyValuePair<string, IState> state in this._states)
            {
                if (state.Value.ShouldCheckBeforeAction(this._currentState))
                {
                    int status = state.Value.CheckBeforeAction(this._currentState);

                    if (status == 0 && state.Value == this._currentState)
                        this._currentState = this.InitialDefaultState();
                    else if (status == 1 && state.Value != this._currentState)
                        this._currentState = state.Value;
                }
            }
        }

        /// <inheritdoc cref="IBot.AfterAction"/>
        public virtual void AfterAction()
        {
            if (this._currentState.IsLocked())
            {
                if (this._currentState.ShouldCheckAfterAction(this._currentState))
                {
                    int status = this._currentState.CheckAfterAction(this._currentState);

                    if (status != 0)
                        this._currentState = this.InitialDefaultState();
                }
                else
                {
                    return;
                }
            }

            if (this._currentState.IsLocked())
                return;

            foreach (KeyValuePair<string, IState> state in this._states)
            {
                if (state.Value.ShouldCheckAfterAction(this._currentState))
                {
                    int status = state.Value.CheckAfterAction(this._currentState);

                    if (status == 0 && state.Value == this._currentState)
                        this._currentState = this.InitialDefaultState();
                    else if (status == 1 && state.Value != this._currentState)
                        this._currentState = state.Value;
                }
            }
        }

        /// <summary>
        /// Whether the bot should start active.
        /// </summary>
        protected virtual bool ShouldStartActive()
        {
            return true;
        }

        /// <summary>
        /// Generates starting set of <see cref="IState">IStates</see>.
        /// </summary>
        /// <returns>Starting set of <see cref="IState">IStates</see></returns>
        protected virtual IDictionary<string, IState> IniticalStates()
        {
            return new Dictionary<string, IState>();
        }

        /// <inheritdoc cref="IniticalStates()"/>
        protected virtual IDictionary<string, IState> IniticalStates(params IState[] states)
        {
            IDictionary<string, IState> dictionary = new Dictionary<string, IState>();

            foreach (IState state in states)
            {
                dictionary.Add(
                    state.GetKey(),
                    state
                );
            }

            return dictionary;
        }

        /// <summary>
        /// Selects the initial <see cref="IState"/> of the bot.
        /// </summary>
        /// <returns></returns>
        protected abstract IState InitialDefaultState();

        /// <summary>
        /// Checks the equivolence of two bots.
        /// </summary>
        /// <param name="other">Other bot.</param>
        /// <returns>Whether the bots are equal.</returns>
        public bool Equals(State other)
        {
            return this.GetKey() == other.GetKey();
        }
    }
}
