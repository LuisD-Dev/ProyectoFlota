import { createBrowserRouter } from 'react-router'
import AppLayout from '@/components/layout/AppLayout'
import NotFound from '@/components/shared/NotFound'
import HomePage from '@/features/home/pages/HomePage'
import LoginPage from '@/features/auth/pages/LoginPage'
import ProtectedRoute from '@/routes/ProtectedRoute'
import ActivosPage from '@/features/activos/pages/ActivosPage'
import ActivoDetallePage from '@/features/activos/pages/ActivoDetallePage'

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
                        path: 'activos/:id',
                        element: <ActivoDetallePage />,
                    },
                    {
                        path: 'activos',
                        element: <ActivosPage />,
                    },
                    {
                        index: true,
                        element: <HomePage />,
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