namespace ClusterAudi
{
    // Definizione interfaccia privata generale
    public interface IFeatureInternal
    {
        // Definizione broadcaster privato per ogni feature (comunicazione Feature - UI MonoBehaviour)
        public IBroadcaster FeatureBroadcaster { get; }
    }
}