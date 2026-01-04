# Gebruik een officiële Node.js runtime
FROM node:18-alpine

# Zet de werkmap in de container
WORKDIR /usr/src/app

# Kopieer package.json en package-lock.json EERST
COPY package*.json ./

# Installeer de dependencies
RUN npm install

# Kopieer de rest van de source code
COPY . .

# Maak de poort van de app beschikbaar
EXPOSE 3000

# Definieer het commando om de app te starten
CMD [ "node", "server.js" ]