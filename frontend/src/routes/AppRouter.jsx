import { createBrowserRouter } from 'react-router'

import AppLayout from '@/components/layout/AppLayout'
import NotFound from '@/components/shared/NotFound'

import HomePage from '@/features/home/pages/HomePage'
import LoginPage from '@/features/auth/pages/LoginPage'

import ActivosPage from '@/features/activos/pages/ActivosPage'
import ActivoDetallePage from '@/features/activos/pages/ActivoDetallePage'

import CategoriasPage from '@/features/categorias/pages/CategoriasPage'

import OrdenesPage from '@/features/ordenes/pages/OrdenesPage'
import OrdenDetallePage from '@/features/ordenes/pages/OrdenDetallePage'

import StockAlertsPage from '@/features/stock/pages/StockAlertsPage'

import ProtectedRoute from '@/routes/ProtectedRoute'

export const router = createBrowserRouter([
    {
        path: '/login',
        element: <LoginPage />,
    },
    {
        element: <ProtectedRoute />,
        children: [
            {
                path: '/',
                element: <AppLayout />,
                children: [
                    {
                        index: true,
                        element: <HomePage />,
                    },
                    {
                        path: 'activos',
                        element: <ActivosPage />,
                    },
                    {
                        path: 'activos/:id',
                        element: <ActivoDetallePage />,
                    },
                    {
                        path: 'categorias',
                        element: <CategoriasPage />,
                    },
                    {
                        path: 'ordenes',
                        element: <OrdenesPage />,
                    },
                    {
                        path: 'ordenes/:id',
                        element: <OrdenDetallePage />,
                    },
                    {
                        path: 'alertas-stock',
                        element: <StockAlertsPage />,
                    },
                ],
            },
        ],
    },
    {
        path: '*',
        element: <NotFound />,
    },
])