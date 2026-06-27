const http = require('http');

const PORT = 55501;
const TARGET_PORT = 55500;

http.createServer((req, res) => {
  console.log(`Proxying: ${req.method} ${req.url}`);
  
  const options = {
    hostname: '127.0.0.1',
    port: TARGET_PORT,
    path: req.url,
    method: req.method,
    headers: { ...req.headers, host: `localhost:${TARGET_PORT}` }
  };

  const proxyReq = http.request(options, (proxyRes) => {
    res.writeHead(proxyRes.statusCode, proxyRes.headers);
    proxyRes.pipe(res, { end: true });
  });

  req.pipe(proxyReq, { end: true });

  proxyReq.on('error', (e) => { 
    console.error(`Proxy Error: ${e.message}`);
    res.writeHead(500); 
    res.end(e.message); 
  });
}).listen(PORT, '0.0.0.0', () => {
  console.log(`✅ Success! Mobile App Proxy running on port ${PORT}`);
  console.log(`Your phone should connect to: http://187.127.178.111:${PORT}`);
});
