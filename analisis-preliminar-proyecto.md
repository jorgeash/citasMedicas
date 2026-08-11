# Proyecto: Sistema de Gestión Clínica — Análisis Preliminar

**Backend:** C# / .NET 9 — Arquitectura Hexagonal — Microservicios
**Frontend:** Vue 3

---

## 1. Alcance del análisis

Se partió de un modelo de dominio de 60 módulos para una clínica (pacientes, citas, expediente clínico, tratamientos, laboratorio, hospitalización, facturación, seguridad, auditoría). Se agrupó todo en **5 microservicios** por dominio funcional, y ahora se define el **frontend en Vue 3** que consumirá esos microservicios.

---

## 2. Backend — Resumen de microservicios

| # | Microservicio | Responsabilidad | Tablas |
|---|---|---|---|
| 1 | **Identity.Service** | Usuarios, roles, permisos, médicos, especialidades, sucursales, horarios | 12 |
| 2 | **Scheduling.Service** | Pacientes, aseguradoras, citas, turnos | 7 |
| 3 | **ClinicalRecords.Service** | Expediente, atenciones, diagnósticos, tratamientos, laboratorio, hospitalización | 30 |
| 4 | **Billing.Service** | Servicios, tarifas, facturación, pagos | 7 |
| 5 | **Notification&Audit.Service** | Notificaciones y auditoría transversal | 2 |

Cada microservicio: base de datos propia, arquitectura hexagonal (Domain → Application → Infrastructure/Persistence → Api), comunicación síncrona (REST/gRPC vía API Gateway) para consultas y asíncrona (eventos) para desacoplar procesos como facturación o notificaciones.

---

## 3. Frontend — Vue 3

### 3.1 Stack tecnológico propuesto

| Capa | Tecnología |
|---|---|
| Framework | Vue 3 (Composition API + `<script setup>`) |
| Build tool | Vite |
| Lenguaje | TypeScript |
| Enrutamiento | Vue Router 4 |
| Estado global | Pinia |
| Cliente HTTP | Axios (con interceptores para JWT y manejo de errores) |
| UI Components | PrimeVue o Vuetify (a definir según preferencia visual) |
| Formularios/Validación | VeeValidate + Zod/Yup |
| Autenticación | JWT emitido por `Identity.Service`, guardado en store de Pinia + interceptor Axios |

### 3.2 Estructura de módulos del frontend (SPA)

El frontend se organiza por **dominio**, reflejando los microservicios pero consumiéndolos todos a través de un único **API Gateway** (un solo `baseURL` para el frontend, el gateway enruta internamente):

```
src/
├── modules/
│   ├── identity/            # login, usuarios, roles, permisos, médicos, especialidades
│   │   ├── views/
│   │   ├── components/
│   │   ├── store/           # Pinia store (auth, usuarios, medicos)
│   │   └── services/        # llamadas API a Identity.Service (via gateway)
│   │
│   ├── scheduling/          # pacientes, aseguradoras, agenda de citas, sala de espera
│   │   ├── views/
│   │   ├── components/
│   │   ├── store/
│   │   └── services/
│   │
│   ├── clinical-records/    # expediente clínico, atención médica, diagnósticos,
│   │   │                    # tratamientos, laboratorio, hospitalización
│   │   ├── views/
│   │   ├── components/
│   │   ├── store/
│   │   └── services/
│   │
│   ├── billing/             # servicios, tarifas, facturación, pagos
│   │   ├── views/
│   │   ├── components/
│   │   ├── store/
│   │   └── services/
│   │
│   └── notifications/       # centro de notificaciones (campana), historial
│       ├── views/
│       ├── components/
│       ├── store/
│       └── services/
│
├── shared/                  # componentes reutilizables (tablas, modales, layout, guards)
├── router/                  # rutas + guards de autenticación/roles
├── api/                     # instancia Axios base + interceptores
└── App.vue / main.ts
```

### 3.3 Pantallas mínimas por módulo 

| Módulo | Pantallas clave |
|---|---|
| Identity | Login, gestión de usuarios, gestión de médicos, catálogo de especialidades |
| Scheduling | Registro de pacientes, agenda de citas (calendario), sala de espera (turnos) |
| ClinicalRecords | Ficha de expediente del paciente, registro de atención (signos vitales, diagnóstico, notas), historial clínico |
| Billing | Generar factura, registrar pago, historial de facturación |
| Notifications | Listado/centro de notificaciones del usuario |

### 3.4 Comunicación Frontend ↔ Backend

- El frontend habla **solo con el API Gateway** (una URL base), nunca directo a cada microservicio — así no le importa cómo está particionado el backend internamente.
- Autenticación: login contra `Identity.Service` → devuelve JWT → se guarda en Pinia (store `auth`) y se adjunta en cada request vía interceptor de Axios.
- Cada módulo del frontend tiene su propio `service` (ej. `scheduling/services/citasService.ts`) que llama a los endpoints correspondientes del gateway — mantiene la misma separación por dominio que el backend.

---

## 4. Alcance sugerido

Dado que el modelo completo (60 tablas / 5 microservicios):

1. **Implementar completo:** 3 microservicios mínimo (ej. Identity, Scheduling, ClinicalRecords o Billing) con hexagonal real y comunicación entre ellos (1 llamada síncrona + 1 evento asíncrono ).
2. **Frontend Vue 3:** cubrir los módulos de los microservicios que sí se implementen, con las pantallas mínimas listadas arriba.
3. **El resto del diseño** (los 5 microservicios completos, las 60 tablas) queda documentado como el **análisis y diseño general del sistema** .

Este documento resume el análisis inicial, al completo que se desarrollara despues agregando o quitando elementos del Sistema en general.
