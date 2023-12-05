using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Sinks.InfluxDB;

public class AutoCluster
{
    public void Run() {
        var scenario = BuildScenario();
        StartNode(scenario);
    }

    private void StartNode(ScenarioProps scenario)
    {
        NBomberRunner
            .RegisterScenarios(scenario)
            .LoadInfraConfig("infra-config.json")
            .LoadConfig("auto-cluster-config.json") 
            .WithReportingInterval(TimeSpan.FromSeconds(5))
            .WithReportingSinks(new InfluxDBSink())
            .EnableLocalDevCluster(true)
            .Run();
    }

    private ScenarioProps BuildScenario()
    {
        return Scenario.Create("hello_world_scenario", async context =>
            {
                await Task.Delay(500);
                return Response.Ok();
            })
            .WithoutWarmUp()
            .WithLoadSimulations(
                Simulation.RampingInject(rate: 20, 
                                         interval: TimeSpan.FromSeconds(1),
                                         during: TimeSpan.FromSeconds(10)),
                
                Simulation.Inject(rate: 20,
                                  interval: TimeSpan.FromSeconds(1),
                                  during: TimeSpan.FromSeconds(30))
                );
    }
}
