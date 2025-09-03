namespace ClusterAudi
{
    public interface IState
    {
        // Ogni stato deve implementare questi metodi che verranno chiamati durante l'esecuzione
        public void StateOnEnter();
        public void StateOnExit();
        public void StateOnUpdate();
    }
}
