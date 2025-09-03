using ClusterAudiFeatures;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
	{
		public class ClusterStartUpFlow
		{
			// Inizializzazione 
			public void BeginStartUp(Client client)
			{
				Debug.Log("[CLUSTER STARTUP] Inizio procedura di avvio cluster...");

				// StartUpFlowTask(client);
				_ = StartUpFlowTask(client); // "_" trovato su internet per evitare warning - messo a fine sviluppo
			}
			
			private async Task StartUpFlowTask(Client client)
			{
				try
				{
					Debug.Log("[CLUSTER STARTUP] Esecuzione flusso di avvio asincrono...");

					// 1. Ottieni servizi necessari
					IBroadcaster clientBroadcaster = client.Services.Get<IBroadcaster>();
					IVehicleDataService vehicleDataService = client.Services.Get<IVehicleDataService>();

					// Utilizzo await per caricamento step a step

					// 2. Configura dati veicolo iniziali
					await InitializeVehicleData(vehicleDataService);

					// 3. Avvia State Machine del cluster
					await InitializeStateMachine(client, clientBroadcaster);

					// 4. Carica tutte le features persistenti
					await LoadPersistentFeatures(client);

					Debug.Log("[CLUSTER STARTUP] Avvio cluster completato con successo!");
				}
				catch (System.Exception ex)
				{
					Debug.LogError($"[CLUSTER STARTUP] Errore durante avvio: {ex.Message}");
				}
			}

			private async Task InitializeVehicleData(IVehicleDataService vehicleDataService)
			{
				Debug.Log("[CLUSTER STARTUP] Inizializzazione dati veicolo...");

				await Task.Delay(500); // Simula caricamento della barra

				// Impostazione dati iniziali veicolo
				vehicleDataService.SetEngineRunning(true);
				vehicleDataService.SetSpeed(0f);
				vehicleDataService.SetRPM(800f);  
				vehicleDataService.SetGear(0); 
				vehicleDataService.SetDriveMode(DriveMode.Comfort);

				Debug.Log("[CLUSTER STARTUP] Dati veicolo inizializzati");
			}

			private async Task InitializeStateMachine(Client client, IBroadcaster broadcaster)
			{
				Debug.Log("[CLUSTER STARTUP] Inizializzazione State Machine...");

				await Task.Delay(300); // Aspetta

				var stateMachine = new ClusterFSM();

				// Passaggio dati al ClusterStateContext
				var context = new ClusterStateContext 
				{
					Client = client,
					ClusterStateMachine = stateMachine,
					VehicleData = client.Services.Get<IVehicleDataService>(),
					Broadcaster = broadcaster
				};

				// Registra tutti gli stati
				stateMachine.AddState("WelcomeState", new WelcomeState(context));
				stateMachine.AddState("EcoModeState", new EcoModeState(context));
				stateMachine.AddState("ComfortModeState", new ComfortModeState(context));
				stateMachine.AddState("SportModeState", new SportModeState(context));

				Debug.Log("[CLUSTER STARTUP] Stati registrati nella State Machine");

				// Avvia con Welcome State
				stateMachine.GoTo("WelcomeState");
				Debug.Log("[CLUSTER STARTUP] State Machine avviata con WelcomeState");

				// Collega State Machine al Client per Update
				if (client is ClusterClient clusterClient)
				{
					clusterClient.SetStateMachine(stateMachine);
				}

				Debug.Log("[CLUSTER STARTUP] State Machine completamente attiva");
			}

			/// Attraverso private async Task caricamento senza bloccare processo
			private async Task LoadPersistentFeatures(Client client)
			{
				Debug.Log("[CLUSTER STARTUP] Caricamento features persistenti...");

				try
				{
					// Caricamento features in parallelo
					var featureTasks = new[]
					{
					LoadAudioFeature(client),
					LoadClusterDriveModeFeature(client),
					LoadSpeedometerFeature(client),
					LoadAutomaticGearboxFeature(client),
					LoadSeatBeltFeature(client),
					LoadClockFeature(client),
					LoadDoorLockFeature(client),
					LoadLaneAssistFeature(client)

					// NB: WelcomeFeature si istanzia all'avvio dello stato, non qui
				};

					// aspetta che tutte finiscano di caricarsi
					await Task.WhenAll(featureTasks);

					Debug.Log("[CLUSTER STARTUP] Features persistenti caricate");
				}
				catch (System.Exception ex)
				{
					Debug.LogError($"[CLUSTER STARTUP] Errore caricamento features: {ex.Message}");
				}
			}

			// Metodi caricamento Feature attraverso Locator

			private async Task LoadAudioFeature(Client client)
			{
				var audioFeature = client.Features.Get<IAudioFeature>();
				await audioFeature.InstantiateAudioFeature();
			}

			private async Task LoadClusterDriveModeFeature(Client client)
			{
				var clusterDriveModeFeature = client.Features.Get<IClusterDriveModeFeature>();
				await clusterDriveModeFeature.InstantiateClusterDriveModeFeature();
			}

			private async Task LoadSpeedometerFeature(Client client)
			{
				var speedometerFeature = client.Features.Get<ISpeedometerFeature>();
				await speedometerFeature.InstantiateSpeedometerFeature();
			}

			private async Task LoadAutomaticGearboxFeature(Client client)
			{
				var automaticgearboxFeature = client.Features.Get<IAutomaticGearboxFeature>();
				await automaticgearboxFeature.InstantiateAutomaticGearboxFeature();
			}

			private async Task LoadSeatBeltFeature(Client client)
			{
				var seatBeltFeature = client.Features.Get<ISeatBeltFeature>();
				await seatBeltFeature.InstantiateSeatBeltFeature();
			}

			private async Task LoadClockFeature(Client client)
			{
				var clockFeature = client.Features.Get<IClockFeature>();
				await clockFeature.InstantiateClockFeature();
			}

			private async Task LoadDoorLockFeature(Client client)
			{
				var doorLockFeature = client.Features.Get<IDoorLockFeature>();
				await doorLockFeature.InstantiateDoorLockFeature();
			}

			private async Task LoadLaneAssistFeature(Client client)
			{
				var laneAssistFeature = client.Features.Get<ILaneAssistFeature>();
				await laneAssistFeature.InstantiateLaneAssistFeature();
			}

		}
	}