using Saltyfish.Util;

namespace UnityEngine.UI
{
    public enum UriSourceType
    {
        Remote,
        LocalFile,
    }

    [AddComponentMenu("UI/UriImage",60)]
    [DisallowMultipleComponent]
    public class UriImage:Image
    {
        [SerializeField]
        private UriSourceType m_UriType = UriSourceType.Remote;

        [SerializeField]
        private string m_Uri;

        [SerializeField]
        private bool m_LoadOnStart = false;

        [SerializeField]
        private bool m_SetNativeSize = true;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UriSpriteLoader.Instance.EndRequest(this);
        }


        protected override void Start()
        {
            if (m_LoadOnStart)
                LoadSprite();
        }

        public void LoadSpriteFromUri(string uri)
        {
            m_Uri = uri;
            if (m_UriType == UriSourceType.Remote)
            {
                UriSpriteLoader.Instance.DisplayFromRemote(this, m_Uri, m_SetNativeSize);
            }
            else
            {
                UriSpriteLoader.Instance.DisplayFromFilePath(this, m_Uri, m_SetNativeSize);
            }
        }


        private void LoadSprite()
        {
            if(m_UriType == UriSourceType.Remote)
            {
                UriSpriteLoader.Instance.DisplayFromRemote(this, m_Uri, m_SetNativeSize);
            }
            else
            {
                UriSpriteLoader.Instance.DisplayFromFilePath(this, m_Uri, m_SetNativeSize);
            }
        }
    }
}
