using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities.States.Default
{
	public class LoadSceneStateLogic : StateLogic, ISwitchStateCondition
	{
		[SerializeField] private SceneAsset m_sceneToLoad = default;
		[SerializeField] private LoadSceneMode m_loadSceneMode = LoadSceneMode.Additive;

		private AsyncOperation m_loadSceneOperation = null;

		public bool Condition => m_loadSceneOperation != null && m_loadSceneOperation.isDone;

		public override void Activate()
		{
			base.Activate();
			var sceneName = m_sceneToLoad.name;
			m_loadSceneOperation = SceneManager.LoadSceneAsync(sceneName, m_loadSceneMode);
		}
	}
}