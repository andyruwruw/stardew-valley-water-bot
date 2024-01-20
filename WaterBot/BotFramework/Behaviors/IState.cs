using StardewModdingAPI.Events;
using System.Collections.Generic;

namespace BotFramework.Behaviors
{
    /// <summary>
    /// States of the bot which specify various behaviors.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Deactivates this state.
        /// </summary>
        void Activate();

        /// <summary>
        /// Activates this state.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Retrieves the key of the state.
        /// </summary>
        /// <returns>Key of the state.</returns>
        string GetKey();

        /// <summary>
        /// Retrieves the name of the state.
        /// </summary>
        /// <returns>Name of the state.</returns>
        string GetName();

        /// <summary>
        /// List of state behaviors.
        /// </summary>
        /// <returns>State <see cref="IBehavior">Behaviors</see></returns>
        IList<IBehavior> GetBehaviors();

        /// <summary>
        /// Whether this state should check every game tick to activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether a check should be performed.</returns>
        bool ShouldCheckOnGameTick(IState active);

        /// <summary>
        /// Checks on game tick whether state should activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        /// <returns>Whether state should activate.</returns>
        int CheckGameTick(
            IState active,
            object sender,
            UpdateTickedEventArgs args
        );

        /// <summary>
        /// Whether this state should check every day start to activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether a check should be performed.</returns>
        bool ShouldCheckOnDayStart(IState active);

        /// <summary>
        /// Checks on day start whether state should activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        /// <returns>Whether state should activate.</returns>
        int CheckDayStart(
            IState active,
            object sender,
            DayStartedEventArgs args
        );

        /// <summary>
        /// Whether this state should check every button press to activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether a check should be performed.</returns>
        bool ShouldCheckOnButtonPress(IState active);

        /// <summary>
        /// Checks on button pressed whether state should activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        /// <returns>Whether state should activate.</returns>
        int CheckButtonPressed(
            IState active,
            object sender,
            ButtonPressedEventArgs args
        );

        /// <summary>
        /// Whether this state should check every warp to activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether a check should be performed.</returns>
        bool ShouldCheckOnWarp(IState active);

        /// <summary>
        /// Checks on warp whether state should activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="args">Event arguments.</param>
        /// <returns>Whether state should activate.</returns>
        int CheckWarped(
            IState active,
            object sender,
            WarpedEventArgs args
        );

        /// <summary>
        /// Whether this state should check before actions.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether a check should be performed.</returns>
        bool ShouldCheckBeforeAction(IState active);

        /// <summary>
        /// Checks before actions whether state should activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether state should activate.</returns>
        int CheckBeforeAction(IState active);

        /// <summary>
        /// Whether this state should check after actions.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether a check should be performed.</returns>
        bool ShouldCheckAfterAction(IState active);

        /// <summary>
        /// Checks after actions whether state should activate.
        /// </summary>
        /// <param name="active">Current state.</param>
        /// <returns>Whether state should activate.</returns>
        int CheckAfterAction(IState active);

        /// <summary>
        /// Whether activation of this state should display a message.
        /// </summary>
        /// <returns>Whether to display an activation message.</returns>
        bool ShowsActivationMessage();

        /// <summary>
        /// Retrieves activation message.
        /// </summary>
        /// <returns>Activation message.</returns>
        string GetActivationMessage();

        /// <summary>
        /// Retrieves activation icon.
        /// </summary>
        /// <returns>Activation icon.</returns>
        string GetActivationIcon();

        /// <summary>
        /// Whether deactivation of this state should display a message.
        /// </summary>
        /// <returns>Whether to display an deactivation message.</returns>
        bool ShowsDeactivationMessage();

        /// <summary>
        /// Retrieves deactivation message.
        /// </summary>
        /// <returns>Deactivation message.</returns>
        string GetDeactivationMessage();

        /// <summary>
        /// Retrieves deactivation icon.
        /// </summary>
        /// <returns>Deactivation icon.</returns>
        string GetDeactivationIcon();

        /// <summary>
        /// Whether this state is locked, preventing other states from triggering.
        /// </summary>
        /// <returns>Whether this state is locked.</returns>
        bool IsLocked();

        /// <summary>
        /// Checks the equivolence of two states.
        /// </summary>
        /// <param name="other">Other state.</param>
        /// <returns>Whether the states are equal.</returns>
        bool Equals(IState other);
    }
}
