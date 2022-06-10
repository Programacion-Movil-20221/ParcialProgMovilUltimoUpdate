using CarritoCompras.Modelo;
using CarritoCompras.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CarritoCompras.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PageDetalleProducto : ContentPage
	{
        public string nombreGlobalCategoria;
        public Producto oGlobalProducto;
        public PageDetalleProducto (Producto oProducto,string nombreCategoria)
		{
			InitializeComponent ();
            nombreGlobalCategoria = nombreCategoria;
            oGlobalProducto = oProducto;

            ImagenProducto.Source = oProducto.imagen;
            txtNombre.Text = oProducto.nombre;
            txtDescripcion.Text = oProducto.descripcion;
            txtDetalle.Text = oProducto.detalle;
            txtPrecio.Text = string.Format("$ {0}", oProducto.precio.ToString());
		}

        private void TapMenos_Tapped(object sender, EventArgs e)
        {
            int cantidad = Convert.ToInt32(lblCantidad.Text);
            if (cantidad > 1)
            {
                cantidad -= 1;
            }
            lblCantidad.Text = cantidad.ToString();
        }

        private void TapMas_Tapped(object sender, EventArgs e)
        {
            int cantidad = Convert.ToInt32(lblCantidad.Text);
            cantidad += 1;
            lblCantidad.Text = cantidad.ToString();
        }

        private async void BtnAgregarBolsa_Clicked(object sender, EventArgs e)
        {

            bool encontrado = await validarProductoEnCarrito();

            if (encontrado)
            {
                await DisplayAlert("Notificacion", "El producto ya se encuentra en el carrito", "Ok");
                return;
            }


            Bolsa oBolsa = new Bolsa()
            {
                cantidad = Convert.ToInt32(lblCantidad.Text),
                categoria = nombreGlobalCategoria,
                producto = oGlobalProducto
            };
            bool resultado = await ApiServiceFirebase.AgregarAlCarrito(oBolsa);

            if (resultado)
            {
                var DisplayResultado = await DisplayAlert("Notificacion", "Producto agregado al carrito!", "Ir al carrito", "Seguir comprando");
                if (DisplayResultado)
                {
                    var ttt = nameof(PageBolsa).ToString();

                    Routing.RegisterRoute(nameof(PageBolsa), typeof(PageBolsa));
                    await Shell.Current.GoToAsync(nameof(PageBolsa));
                }

            }
            else
            {
                await DisplayAlert("Notificacion", "No se pudo agregar al carrito","Ok");
            }
            
        }

        private async Task<bool> validarProductoEnCarrito()
        {
            bool encontrado = false;
            Dictionary<string, Bolsa> oObjecto = await ApiServiceFirebase.ObtenerCarrito();
            if (oObjecto !=null)
            {
                foreach (KeyValuePair<string, Bolsa> item in oObjecto)
                {
                    if (item.Value.producto.idproducto == oGlobalProducto.idproducto)
                    {
                        encontrado = true;
                        break;
                    }
                }
            }
            
            return encontrado;

        }

    }
}