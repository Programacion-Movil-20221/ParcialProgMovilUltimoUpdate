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
	public partial class PagePago : ContentPage
	{
        private Compra oGlobalCompra = new Compra();
		public PagePago (Compra oCompra)
		{
			InitializeComponent ();
            oGlobalCompra = oCompra;

            obtenerEmail();

        }

        private async void BtnConfirmarPago_Clicked(object sender, EventArgs e)
        {
            float precioTotal = 0;

            if ( string.IsNullOrWhiteSpace(txtNumeroTarjeta.Text) || string.IsNullOrWhiteSpace(txtFechaMessExpiracion.Text) || string.IsNullOrWhiteSpace(txtFechaAñoExpiracion.Text) || string.IsNullOrWhiteSpace(txtCodigoCVV.Text) || string.IsNullOrWhiteSpace(txtEmail.Text)) 
            {
                await DisplayAlert("Notificacion", "Todos los campos son obligatorios", "Ok");
                return;
            }

            DetallePago oDetallePago = new DetallePago()
            {
                numeroTarjeta = txtNumeroTarjeta.Text,
                fechaExpiracion = string.Format("{0}/{1}",txtFechaMessExpiracion.Text,txtFechaAñoExpiracion.Text),
                codigoCVV = txtCodigoCVV.Text,
                email = txtEmail.Text
            };

            foreach(Bolsa item in oGlobalCompra.oListaBolsa)
            {
                precioTotal = precioTotal + item.montoTotal;
            }


            oGlobalCompra.oDetallePago = oDetallePago;
            oGlobalCompra.precioTotal = precioTotal;

            bool respuesta = await ApiServiceFirebase.RegistrarCompra(oGlobalCompra);

            if (respuesta)
            {
                await DisplayAlert("Mensaje", "La compra fue realizada con exito!", "Ok");
                App.Current.MainPage = new PageInicio();
            }
            else
            {
                await DisplayAlert("Notificacion", "No se pudo completar la compra", "Ok");
            }

        }

        private async void obtenerEmail()
        {
            Usuario oUsuario = await ApiServiceFirebase.ObtenerUsuario();
            txtEmail.Text = oUsuario.Email;
        }
       
    }
}