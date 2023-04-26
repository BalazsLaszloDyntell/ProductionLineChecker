# ProductionLineChecker

Notificationservice

dapr run --app-id notificationservice --app-port 6001 --dapr-http-port 3601 --dapr-grpc-port 60001 --config ../dapr/config/config.yaml --components-path ../dapr/components dotnet run

SignalProcessService

dapr run --app-id signalprocessservice --app-port 6000 --dapr-http-port 3600 --dapr-grpc-port 60000 --config ../dapr/config/config.yaml --components-path ../dapr/components dotnet run
