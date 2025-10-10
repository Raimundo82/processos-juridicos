import { defineConfig } from 'vitest/config';

export default defineConfig({
    test: {
        environment: 'jsdom',
        coverage: {
            provider: 'v8',
            reporter: ['text', 'lcov', 'html'],
            reportsDirectory: './coverage',
            include: ['Processos-Juridicos/wwwroot/js'],
            exclude: ['**/*.test.{js,ts}', '**/lib/**']
        }
    }
});
