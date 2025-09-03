using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	public interface ILaneAssistFeature : IFeature
	{
		// Istanzia la UI
		Task InstantiateLaneAssistFeature();

		// Abilita/disabilita sistema
		void SetLaneAssistEnabled(bool enabled);

		// Stato sistema
		LaneAssistData.LaneAssistState GetCurrentState();
	}
}