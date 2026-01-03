#!/bin/sh

mkdir -p /etc/nginx/certs

if [ ! -f /etc/nginx/certs/selfsigned.crt ]; then
    echo "Generowanie certyfikatu SSL..."
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
        -keyout /etc/nginx/certs/selfsigned.key \
        -out /etc/nginx/certs/selfsigned.crt \
        -subj "/C=PL/ST=WestPomerania/L=Szczecin/O=IPZ/CN=localhost"
fi

exec nginx -g "daemon off;"