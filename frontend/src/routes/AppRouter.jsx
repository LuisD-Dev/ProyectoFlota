import { createBrowserRouter } from 'react-router'
import AppLayout from '@/components/layout/AppLayout'
import NotFound from '@/components/shared/NotFound'
import HomePage from '@/features/home/pages/HomePage'

export const router = createBrowserRouter([
    {
        path: '/',
        element: <AppLayout />,
        children: [
            {
                index: true,
                element: <HomePage />,
            },
        ],
    },
    {
        path: '*',
        element: <NotFound />,
    },
])