﻿using UnityEngine;
using System.Collections.Generic;

namespace DaburuTools
{
	namespace Action
	{
		public class TickAction : Action
		{
			private static void EmptyFunc() {}
			public delegate void OnActionTickDelegate();
			public OnActionTickDelegate OnActionTick = EmptyFunc;
			float mfActionDuration;
			float mfElaspedDuration;

			public TickAction()
			{
				SetAction(0f);
				SetupAction();
			}
			public TickAction(float _actionDuration, OnActionTickDelegate _onActionTick)
			{
				SetAction(_actionDuration);
				OnActionTick += _onActionTick;
				SetupAction();
			}

			public void SetAction(float _actionDuration)
			{
				mfActionDuration = _actionDuration;
			}
			private void SetupAction()
			{
				mfElaspedDuration = 0f;
			}
			protected override void OnActionBegin()
			{
				base.OnActionBegin();

				SetupAction(); 
			}



			public override void RunAction()
			{
				base.RunAction();

				OnActionTick();
				mfElaspedDuration += ActionDeltaTime(mbIsUnscaledDeltaTime);

				// Remove self after action is finished.
				if (mfElaspedDuration > mfActionDuration)
				{
					OnActionEnd();
					mParent.Remove(this);
				}
			}
			public override void MakeResettable(bool _bIsResettable)
			{
				base.MakeResettable(_bIsResettable);
			}
			public override void Reset()
			{
				SetupAction();
			}
			public override void StopAction(bool _bSnapToDesired)
			{
				if (!mbIsRunning)
					return;

				// Prevent it from Resetting.
				MakeResettable(false);

				// Simulate the action has ended. Does not really matter by how much.
				mfElaspedDuration = mfActionDuration;

				// No need for snap to desired, true same effect as false.
				// Only delays time, will simply run the next Action either ways.

				OnActionEnd();
				mParent.Remove(this);
			}
		}
	}
}
