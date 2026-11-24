import { RouterProvider } from 'react-router'
import { router } from '@/routes/AppRouter'
import { Toaster } from '@/components/ui/sonner'

export default function App() {
  return (
    <>
      <RouterProvider router={router} />
      <Toaster />
    </>
  )
}