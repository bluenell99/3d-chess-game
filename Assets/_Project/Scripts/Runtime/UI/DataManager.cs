namespace ChessGame
{
    public class DataManager : Singleton<DataManager>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        public bool PlayingAgainstBot { get; set; }
    }
}