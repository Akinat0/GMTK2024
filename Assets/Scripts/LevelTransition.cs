using DG.Tweening;
using UnityEngine.SceneManagement;

namespace UnityEngine.UI
{
    public class LevelTransition : MonoBehaviour
    {
        static LevelTransition instance;

        static LevelTransition Instance
        {
            get
            {
                if (instance == null)
                {
                    LevelTransition prefab = Resources.Load<LevelTransition>("LevelTransition");
                    LevelTransition levelTransition = Instantiate(prefab);

                    instance = levelTransition;
                }

                return instance;
            }
        }

        [SerializeField] Image image;

        public static void LoadLevel(string levelName)
        {
            Instance.image.DOFade(1, 0.6f).OnComplete(() => SceneManager.LoadScene(levelName));
        }
        
        public static void LoadLevel(int levelIndex)
        {
            Instance.image.DOFade(1, 0.6f).OnComplete(() => SceneManager.LoadScene(levelIndex));
        }


        void Awake()
        {
            
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoadedHandler;
        }

        void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
        {
            if (mode == LoadSceneMode.Single)
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(0.3f);
                seq.Append(Instance.image.DOFade(0, 0.8f));
            }
        }
    }
}