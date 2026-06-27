#!/bin/bash
mv /root/backend_api/admin_dashboard /var/www/html/admin_dashboard
python3 -c "
import sys
c=open('/etc/nginx/sites-available/default').read()
if 'location /admin_dashboard/' not in c:
    c=c.replace('location / {', 'location /admin_dashboard/ {\n        alias /var/www/html/admin_dashboard/;\n        index index.html;\n    }\n\n    location / {')
    open('/etc/nginx/sites-available/default', 'w').write(c)
"
systemctl reload nginx
