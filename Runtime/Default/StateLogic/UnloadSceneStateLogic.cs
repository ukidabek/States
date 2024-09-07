using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities.States.Default
{
	public class UnloadSceneStateLogic : StateLogic, ISwitchStateCondition
	{
		[SerializeField] private SceneAsset m_sceneToLoad = default;

		private AsyncOperation m_loadSceneOperation = null;

		public bool Condition => m_loadSceneOperation != null && m_loadSceneOperation.isDone;

		public override void Activate()
		{
			base.Activate();
			var sceneName = m_sceneToLoad.name;
			m_loadSceneOperation = SceneManager.UnloadSceneAsync(sceneName);
		}
	}
}