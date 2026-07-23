grafana_compose := grafana-infra.yml
kafka_compose := kafka-compose.yml
s3_compose := s3-compose.yml
redis_compose := redis-compose.yaml
postgres_compose := postgres-db.yaml

help: ## show command list
	@echo "Usage: make <command>"
	@echo ""
	@echo "Commands:"
	@echo ""
	@fgrep -h "##" $(MAKEFILE_LIST) | sed -e 's/\(\:.*\#\#\)/\:\ /' | fgrep -v fgrep | sed -e 's/\\$$//' | sed -e 's/##//'

##

grafana-build: ## build container with auxiliary applications
	docker-compose -f ${grafana_compose} \
				   -f ${s3_compose} build

grafana-up: ## run container with auxiliary applications
	docker-compose -f ${grafana_compose} \
				   -f ${s3_compose} up -d

grafana-down: ## stop container with auxiliary applications
	docker-compose -f ${grafana_compose} \
				   -f ${s3_compose} down
##

s3-build: ## build container with auxiliary applications
	docker-compose -f ${s3_compose} build

s3-up: ## run container with auxiliary applications
	docker-compose -f ${s3_compose} up -d

s3-down: ## stop container with auxiliary applications
	docker-compose -f ${s3_compose} down

##

kafka-build:
	docker-compose -f ${kafka_compose} build

kafka-up:
	docker-compose -f ${kafka_compose} up -d

kafka-down:
	docker-compose -f ${kafka_compose} down

##

redis-build:
	docker-compose -f ${redis_compose} build

redis-up:
	docker-compose -f ${redis_compose} up -d

redis-down:
	docker-compose -f ${redis_compose} down

##

postgres-up:
	docker-compose -f ${postgres_compose} up -d

postgres-down:
	doccker-compose -f ${postgres_compose} down

##
work-infra-build: kafka-build redis-build
work-infra-up: kafka-up redis-up
work-infra-down: kafka-down redis-down
