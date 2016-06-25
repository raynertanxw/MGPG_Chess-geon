using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	namespace Action
	{
		public class CanvasGroupAlphaFadeToAction : Action
		{
			CanvasGroup mCG;
			float mfDesiredAlpha;
			float mfActionDuration;
			Graph mGraph;

			float mfOriginalAlpha;
			float mfElaspedDuration;

			public CanvasGroupAlphaFadeToAction(CanvasGroup _canvasGroup, Graph _graph, float _desiredAlpha, float _actionDuration)
			{
				mCG = _canvasGroup;
				SetGraph(_graph);
				SetDesiredAlpha(_desiredAlpha);
				SetActionDuration(_actionDuration);
				SetGraph(Graph.Linear);
				SetupAction();
			}
			public CanvasGroupAlphaFadeToAction(CanvasGroup _canvasGroup, float _desiredAlpha, float _actionDuration)
			{
				mCG = _canvasGroup;
				SetDesiredAlpha(_desiredAlpha);
				SetActionDuration(_actionDuration);
				SetGraph(Graph.Linear);
				SetupAction();
			}

			public void SetDesiredAlpha(float _newDesiredAlpha)
			{
				mfDesiredAlpha = _newDesiredAlpha;
			}
			public void SetActionDuration(float _newActionDuration)
			{
				mfActionDuration = _newActionDuration;
			}
			public void SetGraph(Graph _newGraph)
			{
				mGraph = _newGraph;
			}
			private void SetupAction()
			{
				mfOriginalAlpha = mCG.alpha;
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

				mfElaspedDuration += ActionDeltaTime(mbIsUnscaledDeltaTime);

				float t = mGraph.Read(mfElaspedDuration / mfActionDuration);
				mCG.alpha = Mathf.Lerp(mfOriginalAlpha, mfDesiredAlpha, t);

				// Remove self after action is finished.
				if (mfElaspedDuration > mfActionDuration)
				{
					// Snap volume to desired volume.
					mCG.alpha = mfDesiredAlpha;

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

				if (_bSnapToDesired)
				{
					// Snap volume to desired volume.
					mCG.alpha = mfDesiredAlpha;
				}

				OnActionEnd();
				mParent.Remove(this);
			}
		}
	}
}
