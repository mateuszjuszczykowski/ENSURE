# Build Docker images
docker build -t api_service:latest_local -f API_service/Dockerfile  .
docker build -t mqtt_broker:latest_local -f MQTT_broker/Dockerfile .
docker build -t mqtt_client:latest_local -f MQTT_client/Dockerfile .
docker build -t data_processor:latest_local -f DATA_processor/Dockerfile .
docker build -t frontend_app:latest_local -f FRONTEND_app/Dockerfile .

# Load images into Kind cluster
kind load docker-image api_service:latest_local --name ensure
kind load docker-image mqtt_broker:latest_local --name ensure
kind load docker-image mqtt_client:latest_local --name ensure
kind load docker-image data_processor:latest_local --name ensure
kind load docker-image frontend_app:latest_local --name ensure

# Apply Kubernetes manifests
kubectl apply -k ./K8s/. --create-namespace ensure