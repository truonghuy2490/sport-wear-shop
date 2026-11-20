#!/usr/bin/env bash
# Docker may source or execute this file depending on its mode inside the container.
(
    set -e
    for password in "$SWS_IDENTITY_PASSWORD" "$SWS_STORE_PASSWORD"; do
        if [[ ! "$password" =~ ^[A-Za-z0-9]{16,128}$ ]]; then
            echo "Database passwords must contain 16–128 ASCII letters or digits." >&2
            exit 1
        fi
    done

    run_sql() {
        if declare -F docker_process_sql >/dev/null; then
            docker_process_sql
        else
            MYSQL_PWD="$MYSQL_ROOT_PASSWORD" mysql --protocol=socket --user=root
        fi
    }

    run_sql <<SQL
CREATE USER 'sws_identity'@'%' IDENTIFIED BY '$SWS_IDENTITY_PASSWORD';
CREATE USER 'sws_store'@'%' IDENTIFIED BY '$SWS_STORE_PASSWORD';
GRANT ALL PRIVILEGES ON \`sportwearshop\_identity\`.* TO 'sws_identity'@'%';
GRANT SELECT, INSERT, UPDATE, DELETE ON \`sportwearshop\_store\`.* TO 'sws_store'@'%';
SQL
)
