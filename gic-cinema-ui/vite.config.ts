import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/cinema': { target: 'https://localhost:7085', changeOrigin: true, secure: false },
      '/cinemalayout': { target: 'https://localhost:7085', changeOrigin: true, secure: false },
      '/bookings': { target: 'https://localhost:7085', changeOrigin: true, secure: false },
      '/reset': { target: 'https://localhost:7085', changeOrigin: true, secure: false },
    }
  }
})
