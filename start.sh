#!/bin/bash

# Dapper SQL Learning - Quick Start Script
# This script will build and start the entire application

set -e

FORCE_REBUILD=false

print_usage() {
    echo "Usage: ./start.sh [--no-cache|rebuild|--rebuild]"
    echo ""
    echo "Options:"
    echo "  --no-cache, rebuild, --rebuild   Force image rebuild without cache"
}

for arg in "$@"; do
    case "${arg}" in
        --no-cache|rebuild|--rebuild)
            FORCE_REBUILD=true
            ;;
        -h|--help)
            print_usage
            exit 0
            ;;
        *)
            echo "Unknown argument: ${arg}" >&2
            print_usage
            exit 1
            ;;
    esac
done

if [ -f .env ]; then
    set -a
    . ./.env
    set +a
fi

PUBLIC_HOST="${PUBLIC_HOST:-localhost}"
PUBLIC_SCHEME="${PUBLIC_SCHEME:-http}"
COMMAND_TIMEOUT="${START_SH_TIMEOUT:-1800}"

APP_BASE_URL="${PUBLIC_SCHEME}://${PUBLIC_HOST}"

run_with_timeout() {
    if command -v timeout > /dev/null 2>&1; then
        timeout "${COMMAND_TIMEOUT}" "$@"
    elif command -v gtimeout > /dev/null 2>&1; then
        gtimeout "${COMMAND_TIMEOUT}" "$@"
    else
        "$@"
    fi
}

is_wsl() {
    grep -qi microsoft /proc/version 2>/dev/null
}

run_windows_podman_from_wsl() {
    if ! command -v podman.exe >/dev/null 2>&1; then
        echo "podman.exe not found in WSL PATH. Cannot run Podman Desktop engine." >&2
        exit 1
    fi

    local compose_cmd
    compose_cmd=(podman.exe compose -f compose.yaml)

    echo "Starting Dapper SQL Learning application..."
    echo ""
    echo "Podman Desktop engine (Windows) is running"
    echo ""
    echo "Stopping existing containers..."
    run_with_timeout "${compose_cmd[@]}" down --remove-orphans || true
    echo ""
    echo "Building API image..."
    if [ "${FORCE_REBUILD}" = "true" ]; then
        echo "Rebuild requested: disabling build cache"
        run_with_timeout podman.exe build --no-cache -t dappertests_api:latest -f DapperSqlLearning.Api/Dockerfile .
    else
        run_with_timeout podman.exe build -t dappertests_api:latest -f DapperSqlLearning.Api/Dockerfile .
    fi
    echo ""
    echo "Starting all services..."
    run_with_timeout "${compose_cmd[@]}" up -d
    echo ""
    echo "Waiting for services to start..."
    sleep 10
    echo "Service Status:"
    run_with_timeout "${compose_cmd[@]}" ps
    echo ""
    echo "--------------------------------------------------"
    echo "SUCCESS! Services are started."
    echo "--------------------------------------------------"
}

if is_wsl; then
    run_windows_podman_from_wsl
    exit $?
fi

# Check if Docker or Podman is running
if command -v podman > /dev/null 2>&1 && podman info > /dev/null 2>&1; then
    CONTAINER_ENGINE="podman"
elif command -v docker > /dev/null 2>&1 && docker info > /dev/null 2>&1; then
    CONTAINER_ENGINE="docker"
else
    echo "Docker/Podman is not running. Please start container engine first." >&2
    exit 1
fi

# Check compose command availability
if [ "${CONTAINER_ENGINE}" = "docker" ]; then
    if docker compose version > /dev/null 2>&1; then
        COMPOSE_CMD=(docker compose)
        COMPOSE_LABEL="docker compose"
    elif command -v docker-compose > /dev/null 2>&1; then
        COMPOSE_CMD=(docker-compose)
        COMPOSE_LABEL="docker-compose"
    else
        echo "Docker Compose is not available. Please install Docker Compose v2+" >&2
        exit 1
    fi
else
    if podman compose version > /dev/null 2>&1; then
        COMPOSE_CMD=(podman compose)
        COMPOSE_LABEL="podman compose"
    elif command -v podman-compose > /dev/null 2>&1; then
        COMPOSE_CMD=(podman-compose)
        COMPOSE_LABEL="podman-compose"
    else
        echo "Podman Compose is not available. Please install podman-compose" >&2
        exit 1
    fi
fi

run_compose() {
    run_with_timeout "${COMPOSE_CMD[@]}" "$@"
}

echo "Starting Dapper SQL Learning application..."
echo ""

if [ "${CONTAINER_ENGINE}" = "docker" ]; then
    ENGINE_LABEL="Docker"
else
    ENGINE_LABEL="Podman"
fi

echo "${ENGINE_LABEL} is running"
echo ""

# Stop existing containers
echo "Stopping existing containers..."
run_compose down 2>/dev/null || true
echo ""

# Build image (FIXED: uses CONTAINER_ENGINE)
echo "Building API image..."
if [ "${FORCE_REBUILD}" = "true" ]; then
    echo "Rebuild requested: disabling build cache"
    run_with_timeout "${CONTAINER_ENGINE}" build --no-cache -t dappertests_api:latest -f DapperSqlLearning.Api/Dockerfile .
else
    run_with_timeout "${CONTAINER_ENGINE}" build -t dappertests_api:latest -f DapperSqlLearning.Api/Dockerfile .
fi
echo ""

# Start services
echo "Starting all services..."
run_compose up -d
echo ""

# Wait for services
echo "Waiting for services to start..."
sleep 10

# Status
echo "Service Status:"
run_compose ps
echo ""

echo "--------------------------------------------------"
echo "SUCCESS! Services are started."
echo "--------------------------------------------------"
echo ""
echo "Access Points:"
echo "   API:            ${APP_BASE_URL}:8080"
echo "   OpenAPI JSON:   ${APP_BASE_URL}:8080/openapi/v1.json"
echo "   pgAdmin:        ${APP_BASE_URL}:5050"
echo ""
echo "Quick Commands:"
echo "   View logs:    ${COMPOSE_LABEL} logs -f"
echo "   Stop all:     ${COMPOSE_LABEL} down"
echo "   Restart:      ${COMPOSE_LABEL} restart"
echo ""