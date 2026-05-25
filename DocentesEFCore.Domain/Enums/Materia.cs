using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DocentesApp.Domain.Enums
{
    public enum Materia
    {
        [Description("Inglés I")]
        InglesI = 1,
        [Description("Inglés II")]
        InglesII = 2,
        [Description("Ingeniería y Sociedad")]
        IngenieriaYSociedad = 3,
        [Description("Economía")]
        Economia = 4,
        [Description("Legislación")]
        Legislacion = 5,
        [Description("Organización y Administración de Empresas")]
        OrgYAdminDeEmpresas = 6,
        [Description("Química General")]
        QuimicaGeneral = 7,
        [Description("Química Aplicada a la Informática")]
        QuimicaAplicadaInform = 8,
        [Description("Física I")]
        FisicaI = 9,
        [Description("Física II")]
        FisicaII = 10,
        [Description("Análisis Matemático I")]
        AnalisisMatematicoI = 11,
        [Description("Análisis Matemático II")]
        AnalisisMatematicoII = 12,
        [Description("Álgebra y Geometría Análítica")]
        AlgebraYGeometria = 13,
        [Description("Probabilidad y Estadística")]
        ProbabilidadYEstadistica = 14,
        [Description("Taller Física")]
        TallerFisica = 15,
        [Description("Taller Matemática")]
        TallerMatematica = 16,
        [Description("Taller Inglés")]
        TallerIngles = 17,
        [Description("Taller Química")]
        TallerQuimica = 18,
        [Description("Laboratorio Física I")]
        LaboratorioFisicaI = 19,
        [Description("Laboratorio Física II")]
        LaboratorioFisicaII = 20,
        [Description("Laboratorio de Informática")]
        LaboratorioInformatica = 21

    }
}
