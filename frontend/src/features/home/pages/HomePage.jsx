import {
    Card,
    CardContent,
    CardHeader,
    CardTitle,
} from '@/components/ui/card'

export default function HomePage() {
    return (
        <Card>
            <CardHeader>
                <CardTitle>Inicio</CardTitle>
            </CardHeader>

            <CardContent>
                <p>Sistema de gestión de mantenimiento de flota.</p>
            </CardContent>
        </Card>
    )
}