using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Utilities.States.Default
{
	public class LoadSceneStateLogic : StateLogic, ISwitchStateCondition
	{
		[SerializeField] private SceneAsset m_sceneToLoad = default;
		[SerializeField] private LoadSceneMode m_loadSceneMode = LoadSceneMode.Additive;
		[SerializeField] private bool m_setSceneAsActive = false;

		private AsyncOperation m_loadSceneOperation = null;

		public bool Condition => m_loadSceneOperation != null && m_loadSceneOperation.isDone;

		public override void Activate()
		{
			base.Activate();
			var sceneName = m_sceneToLoad.name;
			m_loadSceneOperation = SceneManager.LoadSceneAsync(sceneName, m_loadSceneMode);
			if (!m_setSceneAsActive) return;
			if (m_loadSceneOperation == null) return;
			m_loadSceneOperation.completed += OnLoadCompleated;
		}

		private void OnLoadCompleated(AsyncOperation operation)
		{
			operation.completed -= OnLoadCompleated;
			var sceneName = m_sceneToLoad.name;
			var scene = SceneManager.GetSceneByName(sceneName);
			if (!scene.IsValid()) return;
			SceneManager.SetActiveScene(scene);
		}
	}
}