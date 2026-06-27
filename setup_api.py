import os

config_path = '/etc/nginx/sites-available/pmc'
with open(config_path, 'r') as f:
    content = f.read()

if 'location /backend_api/' not in content:
    proxy_block = """
    location /backend_api/ {
        proxy_pass http://127.0.0.1:9000/;
        proxy_set_header Host $host;
    }
"""
    content = content.replace('location / {', proxy_block + '\n    location / {')
    with open(config_path, 'w') as f:
        f.write(content)

os.system('systemctl reload nginx')

dockerfile = """
FROM mono:latest
RUN echo 'deb http://archive.debian.org/debian buster main' > /etc/apt/sources.list && \\
    echo 'deb http://archive.debian.org/debian-security buster/updates main' >> /etc/apt/sources.list && \\
    apt-get update -o Acquire::Check-Valid-Until=false && \\
    apt-get install -y mono-xsp4
WORKDIR /app
COPY . /app
EXPOSE 9000
CMD ["xsp4", "--port", "9000", "--nonstop", "--address", "0.0.0.0"]
"""
with open('/root/backend_api/Dockerfile', 'w') as f:
    f.write(dockerfile)

os.system('cd /root/backend_api && docker build -t pmc_api .')
os.system('docker rm -f pmc_api_server || true')
os.system('docker run -d -p 9000:9000 --name pmc_api_server pmc_api')
