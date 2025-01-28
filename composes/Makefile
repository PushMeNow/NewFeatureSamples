help: ## show command list
	@echo "Usage: make <command>"
	@echo ""
	@echo "Commands:"
	@echo ""
	@fgrep -h "##" $(MAKEFILE_LIST) | sed -e 's/\(\:.*\#\#\)/\:\ /' | fgrep -v fgrep | sed -e 's/\\$$//' | sed -e 's/##//'

grafana-infra-build:
	docker-compose -f grafana-infra.yml build

grafana-infra-up:
	docker-compose -f grafana-infra.yml up -d

grafana-infra-down:
	docker-compose -f grafana-infra.yml down