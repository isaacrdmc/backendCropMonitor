using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CropMonitor.Migrations
{
    /// <inheritdoc />
    public partial class agregacionDeCultivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cultivos",
                columns: new[] { "CultivoID", "Descripcion", "ImagenURL", "Nombre", "RequisitosAgua", "RequisitosClima", "RequisitosLuz" },
                values: new object[,]
                {
                    { 1, "Planta herbácea cultivada para la alimentación. Sus hojas se consumen frescas en ensaladas.", "https://www.guiadejardineria.com/wp-content/uploads/2016/07/cultiva-tus-lechugas-en-maceta-03.jpg", "Lechuga", "Riego frecuente, mantener el suelo húmedo", "Clima fresco, 15-18°C", "Sol parcial" },
                    { 2, "Hortaliza de hoja que crece rápido, ideal para consumo fresco o cocido.", "https://www.minutoar.com.ar/u/fotografias/m/2021/3/18/f768x1-50265_50392_110.jpg", "Espinaca", "Riego regular, evitar el encharcamiento", "Clima fresco, 10-20°C", "Sol parcial o sombra" },
                    { 3, "Planta de hojas grandes, muy nutritiva y resistente. Se usa en guisos y salteados.", "https://www.unhuertoenmibalcon.com/blog/wp-content/uploads/IMG_20130613_210642_web.jpg", "Acelga", "Riego abundante y constante", "Amplia tolerancia, 10-25°C", "Sol directo o semisombra" },
                    { 4, "Una de las hortalizas más nutritivas. Hojas rizadas, ideal para ensaladas o batidos.", "https://www.menudiet.es/images/blog/kale-la-col-rizada-americana.jpg", "Kale", "Riego regular", "Clima fresco, resiste heladas", "Sol directo" },
                    { 5, "Hierba de sabor picante y distintivo. Crece rápidamente, perfecta para ensaladas.", "https://graciasnaturaleza.com/wp-content/uploads/2022/05/plantar-rucula-d.jpg", "Rúcula", "Riego frecuente, evitar que el suelo se seque", "Clima fresco a templado", "Sol completo" },
                    { 6, "Variedad de tomate pequeña y dulce. Perfecta para cultivar en macetas en balcones y terrazas.", "https://huerto-en-casa.com/wp-content/uploads/2022/01/tomate-cherry-en-maceta.jpg", "Tomate cherry", "Riego abundante y constante, especialmente en verano", "Clima cálido, 20-30°C", "Mucho sol" },
                    { 7, "Planta que produce frutos dulces o picantes. Requiere mucho sol para fructificar.", "https://cdn.manomano.com/media/edison/4/7/1/6/47161050dc9c.jpg", "Pimiento", "Riego regular, sin encharcar", "Clima cálido, 20-25°C", "Sol directo" },
                    { 8, "Hortaliza que produce frutos alargados. Necesita espacio y un recipiente grande.", "https://cdn0.ecologiaverde.com/es/posts/4/5/7/cultivar_calabacin_en_maceta_754_orig.jpg", "Calabacín", "Riego muy abundante", "Clima cálido, 18-24°C", "Sol directo" },
                    { 9, "Pequeño fruto rojo y dulce. Se adapta bien a macetas colgantes o camas elevadas.", "https://thumbs.dreamstime.com/b/cierre-de-una-planta-fresa-con-jugosas-bayas-rojas-en-olla-terracota-ideal-para-la-jardiner%C3%ADa-el-hogar-comida-saludable-y-temas-385577850.jpg", "Fresa", "Riego regular, mantener el suelo húmedo", "Templado, 15-25°C", "Sol completo" },
                    { 10, "Fruto alargado, crujiente y refrescante. Requiere un tutor para trepar.", "https://www.imporalaska.com/uploads/products/2022/01/pic_1643299016_1643299058.jpg", "Pepino", "Riego muy abundante y constante", "Clima cálido, 20-30°C", "Sol directo" },
                    { 11, "Hierba aromática con hojas verdes y un sabor dulce y picante. Ideal para la cocina italiana.", "https://s1.elespanol.com/2015/06/11/cocinillas/cocinillas_40255977_116187896_425x640.jpg", "Albahaca", "Riego moderado, sin mojar las hojas", "Clima cálido, sensible al frío", "Sol directo" },
                    { 12, "Hierba muy fácil de cultivar y con un aroma refrescante. Se propaga rápidamente.", "https://www.launion.com.mx/images/2025/1600602ab00db37fdab66f3be0de1c98.jpg", "Menta", "Riego abundante", "Clima templado, resiste el frío", "Sol parcial" },
                    { 13, "Hierba aromática con hojas finas y un sabor fresco. Muy utilizada en la cocina.", "https://s1.elespanol.com/2021/07/07/actualidad/594701747_194475799_1706x960.jpg", "Perejil", "Riego regular, mantener la humedad", "Clima templado", "Semisombra" },
                    { 14, "Hierba de sabor intenso y fresco, común en la cocina latinoamericana y asiática.", "https://cdn0.uncomo.com/es/posts/0/3/7/como_sembrar_cilantro_27730_orig.jpg", "Cilantro", "Riego moderado", "Clima templado a fresco", "Sol parcial" },
                    { 15, "Arbusto aromático de hojas perennes. Muy resistente, ideal para principiantes.", "https://s1.elespanol.com/2015/07/22/cocinillas/cocinillas_50504957_116199069_1273x1280.jpg", "Romero", "Riego escaso, resiste sequía", "Clima seco y cálido", "Sol directo" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Cultivos",
                keyColumn: "CultivoID",
                keyValue: 15);
        }
    }
}
