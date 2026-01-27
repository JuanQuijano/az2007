# Library App

## Description

The Library App is a console-based library management system built with Python, following clean architecture principles. It provides functionality for searching patrons, viewing their loan history, renewing memberships, and managing book loans including returns and extensions.

## Project Structure

```
library/
├── application_core/
│   ├── entities/
│   │   ├── author.py
│   │   ├── book.py
│   │   ├── book_item.py
│   │   ├── loan.py
│   │   └── patron.py
│   ├── enums/
│   │   ├── loan_extension_status.py
│   │   ├── loan_return_status.py
│   │   └── membership_renewal_status.py
│   ├── interfaces/
│   │   ├── iloan_repository.py
│   │   ├── iloan_service.py
│   │   ├── ipatron_repository.py
│   │   └── ipatron_service.py
│   └── services/
│       ├── loan_service.py
│       └── patron_service.py
├── console/
│   ├── main.py
│   ├── console_app.py
│   ├── console_state.py
│   └── common_actions.py
├── infrastructure/
│   ├── json_data.py
│   ├── json_loan_repository.py
│   ├── json_patron_repository.py
│   └── Json/
│       ├── Authors.json
│       ├── Books.json
│       ├── BookItems.json
│       ├── Patrons.json
│       └── Loans.json
└── tests/
    ├── test_loan_service.py
    └── test_patron_service.py
```

## Key Classes and Interfaces

### Core Entities
- **Author** - Representa un autor de libro con ID y nombre
- **Book** - Representa un libro con título, ISBN y referencia al autor
- **BookItem** - Representa una copia física de un libro con fecha de adquisición y condición
- **Patron** - Representa un miembro de la biblioteca con fechas de membresía
- **Loan** - Representa un préstamo de libro con fecha de préstamo, fecha de vencimiento y fecha de devolución

### Servicios
- **LoanService** - Lógica de negocio para operaciones de préstamo (devolver, extender)
- **PatronService** - Lógica de negocio para operaciones de usuario (renovar membresía)

### Repositorios
- **JsonPatronRepository** - Acceso a datos de usuarios desde almacenamiento JSON
- **JsonLoanRepository** - Acceso a datos de préstamos desde almacenamiento JSON

### Interfaces
- **IPatronRepository** - Contrato para operaciones de datos de usuario
- **ILoanRepository** - Contrato para operaciones de datos de préstamo
- **IPatronService** - Contrato para lógica de negocio de usuario
- **ILoanService** - Contrato para lógica de negocio de préstamo

### Aplicación de Consola
- **ConsoleApp** - Orquestador principal de aplicación con patrón de máquina de estados
- **ConsoleState** - Enumeración que define estados de aplicación (PATRON_SEARCH, PATRON_DETAILS, LOAN_DETAILS, QUIT)
- **CommonActions** - Funciones utilitarias compartidas de consola

### Acceso a Datos
- **JsonData** - Gestor de datos central que carga y persiste datos JSON manteniendo relaciones entre entidades

## Enumeraciones
- **LoanExtensionStatus** - Estados para extensión de préstamos (PENDING, APPROVED, REJECTED)
- **LoanReturnStatus** - Estados para devolución de préstamos (PENDING, RETURNED, OVERDUE)
- **MembershipRenewalStatus** - Estados para renovación de membresía (PENDING, APPROVED, REJECTED)

## Uso

### Requisitos Previos
- Python 3.7+

### Ejecutar la Aplicación

1. Navega al directorio console de la biblioteca:
   ```bash
   cd library/console
   ```

2. Ejecuta la aplicación:
   ```bash
   python main.py
   ```

3. Sigue las indicaciones de la consola para:
   - Buscar usuarios por nombre
   - Ver detalles del usuario e historial de préstamos
   - Devolver libros
   - Extender fechas de vencimiento de préstamo
   - Renovar membresía de usuario

### Ejecutar Pruebas

Desde el directorio raíz de library:
```bash
python -m unittest discover tests
```

## Puntos Destacados de Arquitectura

- **Arquitectura en Capas**: Separación clara entre capa de aplicación, infraestructura y presentación
- **Inyección de Dependencias**: Las dependencias se inyectan explícitamente al inicio de la aplicación
- **Patrón State Machine**: La aplicación de consola usa transiciones de estado para gestionar interacciones del usuario
- **Almacenamiento JSON en Memoria**: Los datos se cargan desde archivos JSON y se mantienen en memoria con vinculación de relaciones
- **Patrón Repository**: El acceso a datos se abstrae a través de interfaces de repositorio

## Licencia

MIT
