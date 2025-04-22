#!/bin/bash
# deploy.sh - Deployment script for blue-green deployment

APP_DIR="$HOME/DevOpsDemo"
PRODUCTION_DIR="$APP_DIR/production"
BLUE_DIR="$PRODUCTION_DIR/blue"
GREEN_DIR="$PRODUCTION_DIR/green"
ACTIVE_LINK="$PRODUCTION_DIR/active"
BUILD_DIR="$APP_DIR/build"
LOG_FILE="$APP_DIR/deployment.log"

# Function to log messages
log_message() {
    echo "[$(date +'%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

# Determine current active environment
if [ "$(readlink -f "$ACTIVE_LINK")" = "$BLUE_DIR" ]; then
    INACTIVE_ENV="$GREEN_DIR"
    ACTIVE_ENV="$BLUE_DIR"
    NEW_ACTIVE="green"
else
    INACTIVE_ENV="$BLUE_DIR"
    ACTIVE_ENV="$GREEN_DIR"
    NEW_ACTIVE="blue"
fi

log_message "Starting deployment. Current active: $ACTIVE_ENV, Target: $INACTIVE_ENV"

# Deploy to inactive environment
log_message "Copying build to $INACTIVE_ENV"
rsync -av --delete "$BUILD_DIR/" "$INACTIVE_ENV/"

# Start application in inactive environment
PORT=5001
if [ "$NEW_ACTIVE" = "green" ]; then
    PORT=5002
fi

log_message "Starting application in $NEW_ACTIVE environment on port $PORT"
cd "$INACTIVE_ENV" && dotnet DevOpsDemo.dll --urls="http://localhost:$PORT" > "$INACTIVE_ENV/app.log" 2>&1 &
PID=$!

# Wait for application to start
sleep 5

# Check if application is running
if ps -p $PID > /dev/null; then
    log_message "Application started successfully with PID: $PID"
    
    # Health check
    HEALTH_CHECK=$(curl -s -o /dev/null -w "%{http_code}" http://localhost:$PORT)
    
    if [ "$HEALTH_CHECK" = "200" ]; then
        log_message "Health check passed. Switching active environment to $NEW_ACTIVE"
        
        # Stop current active application
        ACTIVE_PID=$(pgrep -f "dotnet $ACTIVE_ENV/DevOpsDemo.dll" || echo "")
        if [ -n "$ACTIVE_PID" ]; then
            log_message "Stopping current active application (PID: $ACTIVE_PID)"
            kill $ACTIVE_PID
        fi
        
        # Switch active link to new environment
        ln -sf "$INACTIVE_ENV" "$ACTIVE_LINK"
        log_message "Deployment completed successfully. New active environment: $NEW_ACTIVE"
    else
        log_message "Health check failed with status code: $HEALTH_CHECK. Rolling back."
        kill $PID
    fi
else
    log_message "Failed to start application. Deployment aborted."
fi

#!/bin/bash
# health_check.sh - Monitor application health

APP_DIR="$HOME/DevOpsDemo"
ACTIVE_LINK="$APP_DIR/production/active"
LOG_FILE="$APP_DIR/health_check.log"
URL="http://localhost:5000"

log_message() {
    echo "[$(date +'%Y-%m-%d %H:%M:%S')] $1" | tee -a "$LOG_FILE"
}

while true; do
    # Get HTTP status code
    STATUS=$(curl -s -o /dev/null -w "%{http_code}" $URL)
    
    if [ "$STATUS" = "200" ]; then
        log_message "Health check passed: HTTP $STATUS"
    else
        log_message "Health check FAILED: HTTP $STATUS. Attempting to restart application."
        
        # Find the PID of the running application
        APP_PID=$(pgrep -f "dotnet $(readlink -f $ACTIVE_LINK)/DevOpsDemo.dll" || echo "")
        
        if [ -n "$APP_PID" ]; then
            log_message "Stopping application with PID: $APP_PID"
            kill $APP_PID
            sleep 2
        fi
        
        # Restart the application
        log_message "Restarting application"
        cd "$(readlink -f $ACTIVE_LINK)" && dotnet DevOpsDemo.dll --urls="http://localhost:5000" > "$(readlink -f $ACTIVE_LINK)/app.log" 2>&1 &
        NEW_PID=$!
        
        log_message "Application restarted with PID: $NEW_PID"
    fi
    
    # Wait before next check
    sleep 60
done