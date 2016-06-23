using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DaburuTools
{
	namespace Action
	{
		public class ImageAlphaFadeToAction : Action
		{
			Image mImage;
			float mfDesiredAlpha;
			float mfActionDuration;
			Graph mGraph;

			float mfOriginalAlpha;
			float mfElaspedDuration;

			public ImageAlphaFadeToAction(Image _image, Graph _graph, float _desiredAlpha, float _actionDuration)
			{
				mImage = _image;
				SetGraph(_graph);
				SetDesiredAlpha(_desiredAlpha);
				SetActionDuration(_actionDuration);
				SetGraph(Graph.Linear);
				SetupAction();
			}
			public ImageAlphaFadeToAction(Image _image, float _desiredAlpha, float _actionDuration)
			{
				mImage = _image;
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
				mfOriginalAlpha = mImage.color.a;
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
				Color desiredColorWithAlpha = mImage.color;
				desiredColorWithAlpha.a = Mathf.Lerp(mfOriginalAlpha, mfDesiredAlpha, t);
				mImage.color = desiredColorWithAlpha;

				// Remove self after action is finished.
				if (mfElaspedDuration > mfActionDuration)
				{
					// Snap volume to desired volume.
					desiredColorWithAlpha.a = mfDesiredAlpha;
					mImage.color = desiredColorWithAlpha;

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
					Color desiredColorWithAlpha = mImage.color;
					desiredColorWithAlpha.a = mfDesiredAlpha;
					mImage.color = desiredColorWithAlpha;
				}

				OnActionEnd();
				mParent.Remove(this);
			}
		}
	}
}
