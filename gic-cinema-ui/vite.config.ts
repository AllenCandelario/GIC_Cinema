import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/cinema': { target: 'http://localhost:5135', changeOrigin: true, secure: false },
      '/cinemalayout': { target: 'http://localhost:5135', changeOrigin: true, secure: false },
      '/bookings': { target: 'http://localhost:5135', changeOrigin: true, secure: false },
      '/reset': { target: 'http://localhost:5135', changeOrigin: true, secure: false },
    }
  }
})
